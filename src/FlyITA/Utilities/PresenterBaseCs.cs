using System.Runtime.InteropServices;
using PCentralLib;
using PCentralLib.custom_fields;
using PCentralLib.participants;
using PCentralLib.parties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ITALib;
using PCentralLib.programs;

public abstract class PresenterBase
{
    protected IPageViewBase _View;

    protected void SetCustomFieldParticipantIDToCurrentGuestParticipantID()
    {
        CustomFieldControlCollection cfcCollection = default(CustomFieldControlCollection);

        cfcCollection = _View.CustomFieldControls;

        if (cfcCollection.Count > 0)
        {
            foreach (ICustomFieldControl ctl in cfcCollection)
            {
                ctl.ParticipantID = ContextManager.CurrentGuestParticipantID;
            }
        }
    }

    protected void SavePartyCustomFields(PCentralValidationResults ValidateResult, int party_id = 0)
    {
        if (_View.CustomFieldControls.Count > 0)
        {
            if (party_id == 0) party_id = ContextManager.PartyID;

            PCentralParty party = PCentralParty.GetByID(Utilities.GetEnvironmentConnection(), party_id);

            if (party != null)
            {
                foreach (PCentralParticipant p in party.Participants) { SetCustomFieldControls(p, ref ValidateResult); }
            }
        }
    }

    protected void SetCustomFieldControls(PCentralParty party, ref PCentralValidationResults ValidateResult, bool save = true)
    {
        CustomFieldControlCollection cfcCollection = _View.CustomFieldControls; // Get all of the custom field controls on the page
        CustomFieldControlCollection cfcInvalidControls = null;

        Dictionary<int, string> ParticipantIDs = null;
        DataTable dt;
        DataView dvCustomFields = null;
        DataRow dr = null;

        string allConditions = string.Empty;
        int conditionID = 0;
        int participant_count = 0;

        Dictionary<int, string> ProgramCFs = null;

        if (cfcCollection.Count > 0)
        {
            // we may be saving for multiple participants
            ParticipantIDs = new Dictionary<int, string>();

            foreach (ICustomFieldControl ctl in cfcCollection)
            {
                if (ctl.GetCustomFieldID(look_up_if_missing: false) == 0)
                {
                    if (ProgramCFs == null)
                    {
                        ProgramCFs = PCentralProgramCustomField.GetCustomFieldNamesByProgramID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID);
                    }

                    if (ProgramCFs.ContainsValue(ctl.CustomFieldName))
                    {
                        ctl.CustomFieldID = ProgramCFs.FirstOrDefault(x => x.Value == ctl.CustomFieldName).Key;
                    }
                }

                if (!ParticipantIDs.ContainsKey(ctl.ParticipantID))
                {
                    PCentralParticipant P = party.Participants.GetItemByID(ctl.ParticipantID);

                    if (P == null)
                    {
                        ParticipantIDs.Add(ctl.ParticipantID, "your guest");
                    }
                    else
                    {
                        ParticipantIDs.Add(ctl.ParticipantID, P.PersonName);
                    }

                    participant_count++;
                }
            }

            dvCustomFields = new DataView();

            Hashtable isRequired = new Hashtable();

            // Take an initial loop through the custom fields to look for conditions
            // Don't do any of this if there are no custom fields with attributes.
            if (CustomFieldsCache.CountOfCustomFieldsInConfig() > 0)
            {
                string reqCF = null;
                string reqConditions = null;
                string cfName = null;

                // STEP 1:  
                // - Look at each of the entries in CustomFields.config
                // - If there are attributes [requiredif_cf] and [requiredf_list]
                ////		then store that custom field into the "maybeRequired" hashtable.
                Hashtable maybeRequired = new Hashtable();
                Hashtable cfAttribs2 = new Hashtable();

                foreach (string cfName2 in CustomFieldsCache.CurrentCFCache.Keys)
                {
                    reqCF = CustomFieldsCache.GetCustomFieldAttribute(cfName2, "requiredif_cf");
                    reqConditions = CustomFieldsCache.GetCustomFieldAttribute(cfName2, "requiredif_list");

                    if (reqCF.Length > 0 && reqConditions.Length > 0)
                    {
                        cfAttribs2 = new Hashtable();

                        if (maybeRequired.ContainsKey(reqCF))
                        {
                            cfAttribs2.Add("cftorequire", string.Concat(((Hashtable)maybeRequired[reqCF])["cftorequire"].ToString(), "á", cfName2));
                            cfAttribs2.Add("conditions", string.Concat(((Hashtable)maybeRequired[reqCF])["conditions"].ToString(), "á", reqConditions));
                            maybeRequired.Remove(reqCF);
                        }
                        else
                        {
                            cfAttribs2.Add("cftorequire", cfName2);
                            cfAttribs2.Add("conditions", reqConditions);
                        }

                        maybeRequired.Add(reqCF, cfAttribs2);
                    }
                }

                // STEP 2:
                // - Look at each custom field on the page
                // - If the value of that custom field may cause another custom
                ////		field to be required then look at the conditions and try
                ////		to find a match.  If there is a match, put the name of
                ////		the REQUIRED custom field into the "isRequired" hashtable.
                foreach (ICustomFieldControl ctl in cfcCollection)
                {
                    cfName = ctl.CustomFieldName;

                    // If this custom field drives the required behavior of another field
                    if (maybeRequired.ContainsKey(cfName))
                    {
                        allConditions = ((Hashtable)maybeRequired[cfName])["conditions"].ToString();
                        conditionID = 0;

                        // A custom field can have multiple fields that rely on it's value
                        foreach (string uniqueCondition in allConditions.Split(new[] { 'á' }))
                        {
                            // Loop through each condition.  If there is a match, that CF is required
                            if (ctl.IsTextBox)
                            {
                                foreach (string condition in uniqueCondition.Split(new[] { '|' }))
                                {
                                    if (ctl.Value == condition)
                                    {
                                        reqCF = ((Hashtable)maybeRequired[cfName])["cftorequire"].ToString();
                                        reqCF = reqCF.Split(new[] { 'á' })[conditionID];
                                        isRequired.Add(reqCF, true);
                                        break;
                                    }
                                }
                            }
                            else if (ctl.IsDropDown)
                            {
                                foreach (string condition in uniqueCondition.Split(new[] { '|' }))
                                {
                                    if (ctl.CustomFieldValue == condition)
                                    {
                                        reqCF = ((Hashtable)maybeRequired[cfName])["cftorequire"].ToString();
                                        reqCF = reqCF.Split(new[] { 'á' })[conditionID];
                                        isRequired.Add(reqCF, true);
                                        break;
                                    }
                                }
                            }

                            conditionID++;
                        }
                    }
                }
            }

            foreach (int pax_id in ParticipantIDs.Keys)
            {
                // Get back only the custom fields for the current participant
                dt = PCentralCustomFieldDAL.ReadByProgramIDIncludePossibleValues(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID, pax_id, ContextManager.PartyID).Tables[0];

                // attempt to detect errors before saving begins in order to set ValidationMode and prevent a partial save
                dvCustomFields = dt.DataSet.Tables[0].DefaultView;
                dvCustomFields.Sort = "CustomFieldID";
                cfcInvalidControls = new CustomFieldControlCollection();

                foreach (ICustomFieldControl ctl in cfcCollection)
                {
                    if (ctl.ParticipantID == pax_id)
                    {
                        try
                        {
                            if (ctl.IsRequired || isRequired.ContainsKey(ctl.CustomFieldName))
                            {
                                dr = dvCustomFields.FindRows(ctl.CustomFieldID)[0].Row;

                                if (Convert.ToInt32(dr["WebRegTextFlg"]) == 0)
                                {
                                    if (participant_count > 1)
                                    {
                                        Utilities.CheckRequired(ctl.CustomFieldPossibleValueID, ctl.UILabel, ref ValidateResult, " for " + ParticipantIDs[pax_id] + ".");
                                    }
                                    else
                                    {
                                        Utilities.CheckRequired(ctl.CustomFieldPossibleValueID, ctl.UILabel, ref ValidateResult);
                                    }

                                    party.Participants.GetItemByID(pax_id).CustomField_SetValue(ctl.CustomFieldName, ctl.SelectedItemText);
                                }
                                else
                                {
                                    if (participant_count > 1)
                                    {
                                        Utilities.CheckRequired(ctl.Value, ctl.UILabel, ref ValidateResult, " for " + ParticipantIDs[pax_id] + ".");
                                    }
                                    else
                                    {
                                        Utilities.CheckRequired(ctl.Value, ctl.UILabel, ref ValidateResult);
                                    }

                                    party.Participants.GetItemByID(pax_id).CustomField_SetValue(ctl.CustomFieldName, ctl.CustomFieldValue);
                                }
                            }
                            else
                            {
                                dr = dvCustomFields.FindRows(ctl.CustomFieldID)[0].Row;

                                if (Convert.ToInt32(dr["WebRegTextFlg"]) == 0)
                                {
                                    party.Participants.GetItemByID(pax_id).CustomField_SetValue(ctl.CustomFieldName, ctl.SelectedItemText);
                                }
                                else
                                {
                                    party.Participants.GetItemByID(pax_id).CustomField_SetValue(ctl.CustomFieldName, ctl.CustomFieldValue);
                                }
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            cfcInvalidControls.Add(ctl);
                        }
                    }
                }

                // I'm not sure this should happen here...
                if (save && party.Participants.GetItemByID(pax_id).CustomFields.Count > 0)
                {
                    party.Participants.GetItemByID(pax_id).CustomFields.Save(Guid.NewGuid(), ContextManager.RealUserID);
                }
            }
        }
    }

    /// <summary>
    /// Save custom field values
    /// </summary>
    /// <param name="participant"></param>
    /// <param name="ValidateResult"></param>
    /// <param name="save"></param>
    protected void SetCustomFieldControls(PCentralParticipant participant, ref PCentralValidationResults ValidateResult, bool save = true)
    {
        CustomFieldControlCollection cfcCollection = _View.CustomFieldControls; // Get all of the custom field controls on the page
        CustomFieldControlCollection cfcInvalidControls = null;

        Dictionary<int, string> ParticipantIDs = null;
        DataTable dt;
        DataView dvCustomFields = null;
        DataRow dr = null;

        string allConditions = string.Empty;
        int conditionID = 0;
        int participant_count = 0;

        Dictionary<int, string> ProgramCFs = null;

        if (cfcCollection.Count > 0)
        {
            // we may be saving for multiple participants
            ParticipantIDs = new Dictionary<int, string>();

            foreach (ICustomFieldControl ctl in cfcCollection)
            {
                if (ctl.GetCustomFieldID(look_up_if_missing: false) == 0)
                {
                    if (ProgramCFs == null)
                    {
                        ProgramCFs = PCentralProgramCustomField.GetCustomFieldNamesByProgramID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID);
                    }

                    if (ProgramCFs.ContainsValue(ctl.CustomFieldName))
                    {
                        ctl.CustomFieldID = ProgramCFs.FirstOrDefault(x => x.Value == ctl.CustomFieldName).Key;
                    }
                }

                if (!ParticipantIDs.ContainsKey(ctl.ParticipantID))
                {
                    if (participant.ParticipantID == ctl.ParticipantID)
                    {
                        ParticipantIDs.Add(ctl.ParticipantID, participant.PersonName);
                    }
                    else
                    {
                        PCentralParticipant P = participant.BelongsTo_PartyGuests.GetItemByID(ctl.ParticipantID);

                        ParticipantIDs.Add(ctl.ParticipantID, (P == null ? "your guest" : P.PersonName));
                    }

                    participant_count++;
                }
            }

            dvCustomFields = new DataView();

            Hashtable isRequired = new Hashtable();

            // Take an initial loop through the custom fields to look for conditions
            // Don't do any of this if there are no custom fields with attributes.
            if (CustomFieldsCache.CountOfCustomFieldsInConfig() > 0)
            {
                string reqCF = null;
                string reqConditions = null;
                string cfName = null;

                // STEP 1:  
                // - Look at each of the entries in CustomFields.config
                // - If there are attributes [requiredif_cf] and [requiredf_list]
                ////		then store that custom field into the "maybeRequired" hashtable.
                Hashtable maybeRequired = new Hashtable();
                Hashtable cfAttribs2 = new Hashtable();

                foreach (string cfName2 in CustomFieldsCache.CurrentCFCache.Keys)
                {
                    reqCF = CustomFieldsCache.GetCustomFieldAttribute(cfName2, "requiredif_cf");
                    reqConditions = CustomFieldsCache.GetCustomFieldAttribute(cfName2, "requiredif_list");

                    if (reqCF.Length > 0 && reqConditions.Length > 0)
                    {
                        cfAttribs2 = new Hashtable();

                        if (maybeRequired.ContainsKey(reqCF))
                        {
                            cfAttribs2.Add("cftorequire", string.Concat(((Hashtable)maybeRequired[reqCF])["cftorequire"].ToString(), "á", cfName2));
                            cfAttribs2.Add("conditions", string.Concat(((Hashtable)maybeRequired[reqCF])["conditions"].ToString(), "á", reqConditions));
                            maybeRequired.Remove(reqCF);
                        }
                        else
                        {
                            cfAttribs2.Add("cftorequire", cfName2);
                            cfAttribs2.Add("conditions", reqConditions);
                        }

                        maybeRequired.Add(reqCF, cfAttribs2);
                    }
                }

                // STEP 2:
                // - Look at each custom field on the page
                // - If the value of that custom field may cause another custom
                //		field to be required then look at the conditions and try
                //		to find a match.  If there is a match, put the name of
                //		the REQUIRED custom field into the "isRequired" hashtable.
                foreach (ICustomFieldControl ctl in cfcCollection)
                {
                    cfName = ctl.CustomFieldName;

                    // If this custom field drives the required behavior of another field
                    if (maybeRequired.ContainsKey(cfName))
                    {
                        allConditions = ((Hashtable)maybeRequired[cfName])["conditions"].ToString();
                        conditionID = 0;

                        // A custom field can have multiple fields that rely on it's value
                        foreach (string uniqueCondition in allConditions.Split(new[] { 'á' }))
                        {
                            // Loop through each condition.  If there is a match, that CF is required
                            if (ctl.IsTextBox)
                            {
                                foreach (string condition in uniqueCondition.Split(new[] { '|' }))
                                {
                                    if (ctl.Value == condition)
                                    {
                                        reqCF = ((Hashtable)maybeRequired[cfName])["cftorequire"].ToString();
                                        reqCF = reqCF.Split(new[] { 'á' })[conditionID];
                                        isRequired.Add(reqCF, true);
                                        break;
                                    }
                                }
                            }
                            else if (ctl.IsDropDown)
                            {
                                foreach (string condition in uniqueCondition.Split(new[] { '|' }))
                                {
                                    if (ctl.CustomFieldValue == condition)
                                    {
                                        reqCF = ((Hashtable)maybeRequired[cfName])["cftorequire"].ToString();
                                        reqCF = reqCF.Split(new[] { 'á' })[conditionID];
                                        isRequired.Add(reqCF, true);
                                        break;
                                    }
                                }
                            }

                            conditionID++;
                        }
                    }
                }
            }

            foreach (int pax_id in ParticipantIDs.Keys)
            {
                // Get back only the custom fields for the current participant
                dt = PCentralCustomFieldDAL.ReadByProgramIDIncludePossibleValues(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID, pax_id, ContextManager.PartyID).Tables[0];

                // attempt to detect errors before saving begins in order to set ValidationMode and prevent a partial save
                dvCustomFields = dt.DataSet.Tables[0].DefaultView;
                dvCustomFields.Sort = "CustomFieldID";
                cfcInvalidControls = new CustomFieldControlCollection();

                foreach (ICustomFieldControl ctl in cfcCollection)
                {
                    if (ctl.ParticipantID == pax_id)
                    {
                        try
                        {
                            if (ctl.IsRequired || isRequired.ContainsKey(ctl.CustomFieldName))
                            {
                                dr = dvCustomFields.FindRows(ctl.CustomFieldID)[0].Row;

                                if (Convert.ToInt32(dr["WebRegTextFlg"]) == 0)
                                {
                                    if (participant_count > 1)
                                    {
                                        Utilities.CheckRequired(ctl.CustomFieldPossibleValueID, ctl.UILabel, ref ValidateResult, " for " + ParticipantIDs[pax_id] + ".");
                                    }
                                    else
                                    {
                                        Utilities.CheckRequired(ctl.CustomFieldPossibleValueID, ctl.UILabel, ref ValidateResult);
                                    }

                                    participant.CustomField_SetValue(ctl.CustomFieldName, ctl.SelectedItemText);
                                }
                                else
                                {
                                    if (participant_count > 1)
                                    {
                                        Utilities.CheckRequired(ctl.Value, ctl.UILabel, ref ValidateResult, " for " + ParticipantIDs[pax_id] + ".");
                                    }
                                    else
                                    {
                                        Utilities.CheckRequired(ctl.Value, ctl.UILabel, ref ValidateResult);
                                    }

                                    participant.CustomField_SetValue(ctl.CustomFieldName, ctl.CustomFieldValue);
                                }
                            }
                            else
                            {
                                dr = dvCustomFields.FindRows(ctl.CustomFieldID)[0].Row;

                                if (Convert.ToInt32(dr["WebRegTextFlg"]) == 0)
                                {
                                    //if (ctl.SelectedItemText.ToLower() != "select")
                                    if (!(Utilities.IsNothingNullOrEmptyorZero(ctl.CustomFieldValue)))
                                    {
                                        participant.CustomField_SetValue(ctl.CustomFieldName, ctl.SelectedItemText);
                                    }
                                }
                                else
                                {
                                    if (ctl.CustomFieldValue != "0")
                                    {
                                        participant.CustomField_SetValue(ctl.CustomFieldName, ctl.CustomFieldValue);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            cfcInvalidControls.Add(ctl);
                        }
                    }
                }

                if (participant.CustomFields.Count > 0)
                {
                    foreach (PCentralProgramCustomField CF in participant.CustomFields)
                    {
                        ValidateResult.combine_with(CF.Validate("The value you have entered for {name}, exceeds the maximum field length of {maxlen} characters."));
                    }

                    if (save && ValidateResult.is_valid)
                    {
                        participant.CustomFields.Save(Guid.NewGuid(), ContextManager.RealUserID, validate_items: false);
                    }
                }
            }
        }
    }

    // Get records returns the Entity Collection
    public DataTable GetDataTable(PCentralEntityList<PCentralProgramCustomField> Records)
    {
        PCentralEntityList<PCentralProgramCustomField> accountRecords = Records;
        DataTable dTable = new DataTable();

        if (accountRecords.Count == 0)
        {
            return null;
        }

        dTable.Columns.Add("ParticipantID", typeof(int));
        dTable.Columns.Add("CustomFieldID", typeof(int));
        dTable.Columns.Add("ID", typeof(int));
        dTable.Columns.Add("CustomFieldName", typeof(string));
        dTable.Columns.Add("WebRegWriteOnceFlg", typeof(Boolean));
        dTable.Columns.Add("WebRegTextFlg", typeof(Boolean));
        dTable.Columns.Add("CustomFieldPossibleValueID", typeof(int));
        dTable.Columns.Add("AssignedValue", typeof(string));

        foreach (PCentralProgramCustomField entity in accountRecords)
        {
            DataRow dRow = dTable.NewRow();
            dRow["ParticipantID"] = entity.ParticipantID;
            dRow["CustomFieldID"] = entity.CustomFieldID;
            //dRow["TypeId"] =entity.t;
            dRow["ID"] = entity.ProgramCustomFieldID;
            dRow["CustomFieldName"] = entity.CustomFieldName;
            dRow["WebRegWriteOnceFlg"] = entity.WebRegWriteOnceFlg;
            dRow["WebRegTextFlg"] = entity.WebRegTextFlg;
            dRow["CustomFieldPossibleValueID"] = entity.CustomFieldPossibleValueID;
            dRow["AssignedValue"] = entity.CustomFieldValue;

            dTable.Rows.Add(dRow);
        }

        return dTable;
    }

    protected void LoadCustomFieldControls()
    {
        CustomFieldControlCollection cfcCollection = _View.CustomFieldControls;
        DataTable CustomFieldInfo;
        DataView dv = null;
        List<int> arrParticipantID = null;

        if (cfcCollection.Count > 0)
        {
            // we may be reading for multiple participants
            arrParticipantID = new List<int>();

            foreach (ICustomFieldControl ctl in cfcCollection)
            {
                if (!arrParticipantID.Contains(ctl.ParticipantID))
                {
                    arrParticipantID.Add(ctl.ParticipantID);
                }
            }

            // Load up the cache to learn about the custom fields
            string sort = "";
            string cfName = null;
            string hideCF = null;
            string hideConditions = null;
            string showCF = null;
            string showConditions = null;
            string currentValueOfCF = null;
            Dictionary<string, string> pax_cfs = null;

            foreach (int ParticipantID in arrParticipantID)
            {
                pax_cfs = PCentralProgramCustomField.GetCustomFieldValuesByParticipantID(Utilities.GetEnvironmentConnection(), ParticipantID);

                CustomFieldInfo = PCentralCustomFieldDAL.ReadByProgramIDIncludePossibleValues(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID, ParticipantID, ContextManager.PartyID).Tables[0];

                if (CustomFieldInfo != null)
                {
                    dv = CustomFieldInfo.DefaultView;

                    foreach (ICustomFieldControl ctl in cfcCollection)
                    {
                        cfName = ctl.CustomFieldName;

                        // Should we CONDITIONALLY HIDE the custom field?
                        hideCF = CustomFieldsCache.GetCustomFieldAttribute(cfName, "hideif_cf");

                        if (hideCF.Length > 0)
                        {
                            try
                            {
                                if (pax_cfs.ContainsKey(hideCF)) currentValueOfCF = string.Concat(pax_cfs[hideCF], string.Empty).ToLower();

                                hideConditions = CustomFieldsCache.GetCustomFieldAttribute(cfName, "hideif_list");

                                if (hideConditions.Length > 0)
                                {
                                    // Use the ß (ALT-0223) character to represent a blank space
                                    hideConditions = hideConditions.Replace("ß", string.Empty);

                                    foreach (string condition in hideConditions.Split(new[] { '|' }))
                                    {
                                        // Loop through each condition and hide it if there is a match
                                        if (currentValueOfCF.Contains(condition.ToLower()))
                                        {
                                            ctl.IsRequired = false;
                                            ctl.IsVisible = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // no current value
                                string x = ex.ToString();
                            }
                        }

                        // Should we CONDITIONALLY SHOW the custom field?
                        showCF = CustomFieldsCache.GetCustomFieldAttribute(cfName, "showif_cf");

                        if (showCF.Length > 0)
                        {
                            ctl.IsVisible = false;

                            try
                            {
                                if (pax_cfs.ContainsKey(hideCF)) currentValueOfCF = string.Concat(pax_cfs[showCF], string.Empty).ToLower();

                                showConditions = CustomFieldsCache.GetCustomFieldAttribute(cfName, "showif_list");

                                if (showConditions.Length > 0)
                                {
                                    // Use the ß (ALT-0223) character to represent a blank space
                                    showConditions = showConditions.Replace("ß", string.Empty);

                                    foreach (string condition in showConditions.Split(new[] { '|' }))
                                    {
                                        // Loop through each condition and show it if there is a match
                                        if (currentValueOfCF.Contains(condition.ToLower()))
                                        {
                                            ctl.IsVisible = true;
                                            break;
                                        }
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                // no current value
                                string x = ex.ToString();
                            }

                            // If we didn't end up showing this, make it not required
                            if (!ctl.IsVisible)
                            {
                                ctl.IsRequired = false;
                            }
                        }

                        // Is the custom field OPTIONAL?
                        string isOptional = CustomFieldsCache.GetCustomFieldAttribute(cfName, "optional");
                        if (string.Compare(isOptional, "true", true) == 0)
                        {
                            ctl.IsRequired = false;
                        }

                        // Should we HIDE the custom field?
                        string isHidden = CustomFieldsCache.GetCustomFieldAttribute(cfName, "hidden");
                        if (string.Compare(isHidden, "true", true) == 0)
                        {
                            ctl.IsVisible = false;
                            ctl.IsRequired = false;
                        }

                        if (ctl.ParticipantID == ParticipantID)
                        {
                            // now set the properties
                            dv.RowFilter = string.Concat("CustomFieldName='", cfName.Replace("'", "''"), "'");

                            if (dv.Count > 0)
                            {
                                ctl.CustomFieldID = Convert.ToInt32(dv[0]["CustomFieldID"]);

                                if ((Convert.ToInt32(dv[0]["WebRegWriteOnceFlg"]) != 0) && !((dv[0]["CustomFieldPossibleValueID"].ToString().Length == 0) || (Convert.ToInt32(dv[0]["CustomFieldPossibleValueID"]) == 0)))
                                {
                                    ctl.WriteOnce = true;
                                    ctl.Value = dv[0]["AssignedValue"].ToString();
                                }
                                else if (Convert.ToInt32(dv[0]["WebRegTextFlg"]) == 0)
                                {
                                    // Dropdown
                                    ctl.IsDropDown = true;

                                    if (!ctl.HasDataSource)
                                    {
                                        // try to sort
                                        sort = CustomFieldsCache.GetCustomFieldAttribute(cfName, "sort");
                                        DataTable Dt = PCentralProgramCustomField.GetCustomFieldPossibleValues(Utilities.GetEnvironmentConnection(), ctl.CustomFieldID).Tables[0].Copy();
                                        if (sort.Length > 0)
                                        {
                                            Dt.DefaultView.Sort = sort;
                                        }

                                        ctl.dtCustomFieldPossibleValues = Dt;
                                    }

                                    if (ctl.CustomFieldPossibleValueID == 0)
                                    {
                                        ctl.CustomFieldPossibleValueID = dv[0]["CustomFieldPossibleValueID"].toInt();
                                        /*
                                        if (Utilities.IsNothingNullOrEmpty(dv[0]["CustomFieldPossibleValueID"]))
                                        {
                                            ctl.CustomFieldPossibleValueID = 0;
                                        }
                                        else
                                        {
                                            ctl.CustomFieldPossibleValueID = Convert.ToInt32(dv[0]["CustomFieldPossibleValueID"]);
                                        }
                                        */
                                    }
                                }
                                else
                                {
                                    // Textbox
                                    ctl.IsTextBox = true;
                                    ctl.Value = dv[0]["AssignedValue"].ToString();
                                }
                            }
                            else
                            {
                                ctl.CustomFieldID = 0;
                                ctl.WriteOnce = true;
                                ctl.Value = "<span style=\"color: red; font-weight: bold;\">Invalid Custom Field \"" + ctl.CustomFieldName + "\"</span>";
                            }
                        }
                    }
                }
            }
        }
    }

    //todo - temporarily made the boEventArgs as optional to avoid making changes in all the reference pages- Ahmed
    protected CustomFieldControlCollection SaveCustomFieldControls(bool blnValidationMode, PCentralSaveResults SaveResults)
    {
        PCentralEntityList<PCentralParticipant> participants = new PCentralEntityList<PCentralParticipant>();
        CustomFieldControlCollection cfcCollection = null;
        CustomFieldControlCollection cfcInvalidControls = new CustomFieldControlCollection();
        PCentralValidationResults errors = new PCentralValidationResults();
        DataView dvCustomFields = null;
        DataRow rowOriginal = null;
        List<int> participant_ids = null;
        string allConditions = string.Empty;
        int conditionID = 0;
        DataTable PossibleValues;

        try
        {
            // Get all of the custom field controls on the page
            cfcCollection = _View.CustomFieldControls;

            if (cfcCollection.Count > 0)
            {
                // we may be saving for multiple participants
                participant_ids = new List<int>();

                foreach (ICustomFieldControl ctl in cfcCollection)
                {
                    if (!participant_ids.Contains(ctl.ParticipantID))
                    {
                        participant_ids.Add(ctl.ParticipantID);
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                dvCustomFields = new DataView();

                Hashtable isRequired = new Hashtable();

                // Take an initial loop through the custom fields to look for conditions
                // Don't do any of this if there are no custom fields with attributes.
                if (CustomFieldsCache.CountOfCustomFieldsInConfig() > 0)
                {
                    string reqCF = null;
                    string reqConditions = null;
                    string cfName = null;

                    // STEP 1:  
                    // - Look at each of the entries in CustomFields.config
                    // - If there are attributes [requiredif_cf] and [requiredf_list]
                    ////		then store that custom field into the "maybeRequired" hashtable.
                    Dictionary<string, Dictionary<string, string>> maybeRequired = new Dictionary<string, Dictionary<string, string>>();
                    Dictionary<string, string> cust_field_attributes_2 = new Dictionary<string, string>();

                    foreach (string custom_field_name in CustomFieldsCache.CurrentCFCache.Keys)
                    {
                        reqCF = CustomFieldsCache.GetCustomFieldAttribute(custom_field_name, "requiredif_cf");
                        reqConditions = CustomFieldsCache.GetCustomFieldAttribute(custom_field_name, "requiredif_list");

                        if (reqCF.Length > 0 && reqConditions.Length > 0)
                        {
                            cust_field_attributes_2 = new Dictionary<string, string>();

                            if (maybeRequired.ContainsKey(reqCF))
                            {
                                cust_field_attributes_2.Add("cftorequire", string.Concat(maybeRequired[reqCF]["cftorequire"].ToString(), "á", custom_field_name));
                                cust_field_attributes_2.Add("conditions", string.Concat(maybeRequired[reqCF]["conditions"].ToString(), "á", reqConditions));
                                maybeRequired.Remove(reqCF);
                            }
                            else
                            {
                                cust_field_attributes_2.Add("cftorequire", custom_field_name);
                                cust_field_attributes_2.Add("conditions", reqConditions);
                            }

                            maybeRequired.Add(reqCF, cust_field_attributes_2);
                        }
                    }

                    // STEP 2:
                    // - Look at each custom field on the page
                    // - If the value of that custom field may cause another custom
                    ////		field to be required then look at the conditions and try
                    ////		to find a match.  If there is a match, put the name of
                    ////		the REQUIRED custom field into the "isRequired" hashtable.
                    foreach (ICustomFieldControl ctl in cfcCollection)
                    {
                        cfName = ctl.CustomFieldName;

                        // If this custom field drives the required behavior of another field
                        if (maybeRequired.ContainsKey(cfName))
                        {
                            allConditions = maybeRequired[cfName]["conditions"].ToString();
                            conditionID = 0;

                            // A custom field can have multiple fields that rely on it's value
                            foreach (string uniqueCondition in allConditions.Split(new[] { 'á' }))
                            {
                                // Loop through each condition.  If there is a match, that CF is required
                                if (ctl.IsTextBox)
                                {
                                    foreach (string condition in uniqueCondition.Split(new[] { '|' }))
                                    {
                                        if (ctl.Value == condition)
                                        {
                                            reqCF = maybeRequired[cfName]["cftorequire"].ToString();
                                            reqCF = reqCF.Split(new[] { 'á' })[conditionID];
                                            isRequired.Add(reqCF, true);
                                            break;
                                        }
                                    }
                                }
                                else if (ctl.IsDropDown)
                                {
                                    foreach (string condition in uniqueCondition.Split(new[] { '|' }))
                                    {
                                        if (ctl.CustomFieldValue == condition)
                                        {
                                            reqCF = maybeRequired[cfName]["cftorequire"].ToString();
                                            reqCF = reqCF.Split(new[] { 'á' })[conditionID];
                                            isRequired.Add(reqCF, true);
                                            break;
                                        }
                                    }
                                }

                                conditionID++;
                            }
                        }
                    }
                }

                foreach (int participant_id in participant_ids)
                {
                    PossibleValues = PCentralCustomFieldDAL.ReadByProgramIDIncludePossibleValues(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID, ContextManager.ParticipantID, ContextManager.PartyID).Tables[0];
                    PCentralParticipant participant = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), participant_id);

                    // Get back only the custom fields for the current participant
                    bool success = GetCustomFieldControlData(participant, cfcCollection);

                    if (success)
                    {
                        // attempt to detect errors before saving begins in order to set ValidationMode and prevent a partial save
                        dvCustomFields = PossibleValues.DefaultView;
                        dvCustomFields.Sort = "CustomFieldID";

                        foreach (ICustomFieldControl ctl in cfcCollection)
                        {
                            if (ctl.ParticipantID == participant_id && (ctl.IsRequired || isRequired.ContainsKey(ctl.CustomFieldName)))
                            {
                                try
                                {
                                    rowOriginal = dvCustomFields.FindRows(ctl.CustomFieldID)[0].Row;

                                    if (Convert.ToInt32(rowOriginal["WebRegTextFlg"]) == 0)
                                    {
                                        Utilities.CheckRequired(ctl.CustomFieldPossibleValueID, ctl.UILabel, ref errors);
                                    }
                                    else
                                    {
                                        Utilities.CheckRequired(ctl.Value, ctl.UILabel, ref errors);
                                    }
                                }
                                catch
                                {
                                    cfcInvalidControls.Add(ctl);
                                }
                            }
                        }

                        // remove "invalid" controls from the collection, still allow save of any others that may be valid
                        foreach (ICustomFieldControl ctl in cfcInvalidControls)
                        {
                            cfcCollection.Remove(ctl);
                        }

                        if (errors.errors.Count > 0)
                        {
                            blnValidationMode = true;
                        }

                        participants.Add(participant);
                    }
                }

                SaveResults = participants.Save(Guid.NewGuid(), ContextManager.RealUserID);
            }
        }
        catch { }

        return cfcInvalidControls;
    }

    /// <summary>
    /// Loops through all of the custom field controls on the page and add
    /// them to a collection of business objects if the CF.ParticipantID is
    /// what is passed in.
    /// </summary>
    /// <param name="cfcCollection">Collection of all custom fields on the page</param>
    /// <param name="ParticipantID">Participant ID to match on</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private bool GetCustomFieldControlData(PCentralParticipant pax, CustomFieldControlCollection cfcCollection)
    {
        try
        {
            foreach (ICustomFieldControl ctl in cfcCollection)
            {
                if (ctl.ParticipantID == pax.ParticipantID)
                {
                    pax.CustomField_SetValue(ctl.CustomFieldName, ctl.CustomFieldValue);
                    return true;
                }
            }
        }
        catch { }

        return false;
    }

    /// <summary>
    /// replace current setting with new setting only if new setting is not empty 
    /// </summary>
    /// <param name="current_setting"></param>
    /// <param name="new_setting"></param>
    protected void init_setting(ref string current_setting, string new_setting)
    {
        if (!Utilities.IsNothingNullOrEmpty(new_setting))
        {
            current_setting = new_setting;
        }
    }
    protected void CreateCustomFieldApolloFieldCategory(string customFieldName)
    {
        Dictionary<int, string> ProgramCFs = null;

        ProgramCFs = PCentralProgramCustomField.GetCustomFieldNamesByProgramID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID);

        if ((ProgramCFs.ContainsValue(customFieldName)) || Utilities.IsNothingNullOrEmpty(customFieldName) || ContextManager.IsSelfEnroll)
        {
            return;
        }

        PCentralCustomField customField = new PCentralCustomField(Utilities.GetEnvironmentConnection());

        customField.ProgramID = ContextManager.ProgramID;
        customField.CustomFieldCategoryID = Enumerations.enumCustomFieldCategories.ParticipantInformationType;
        customField.CopyCustomFieldValueToEntirePartyInd = false;
        customField.CustomFieldName = customFieldName;


        PCentralSaveResults pcentralSaveResults = customField.Save(Guid.NewGuid(), ContextManager.RealUserID, true, false);

        if (pcentralSaveResults.was_successful)
        {
            PcentralCustomFieldProgramBehaviors cfpbe =
                PcentralCustomFieldProgramBehaviors.GetByCustomFieldIDProgramID(
                    Utilities.GetEnvironmentConnection(), customField.CustomFieldID, ContextManager.ProgramID);


            if (cfpbe != null)
            {
                cfpbe.data_timestamp = DateTime.Now.toDateTimeNullable();
                cfpbe.WebRegHiddenFlg = true;
                cfpbe.WebRegTextFlg = true;
                cfpbe.WebRegWriteOnceFlg = true;
                cfpbe.CustomFieldID = customField.CustomFieldID;


                PCentralSaveResults cfpbeSaveResults = cfpbe.Save(Guid.NewGuid(), ContextManager.RealUserID, true,
                    false);

            }


        }



    }

}