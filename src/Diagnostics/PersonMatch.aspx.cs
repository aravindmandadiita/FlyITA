using PCentralLib;
using PCentralLib.person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ITALib;
using System.Data;
using PCentralLib.email;
using ITALib.DataAccess;

namespace FlyITA.Diagnostics
{
    public partial class PersonMatch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PCentralCodeLookup.iso_gender_codes(Utilities.GetEnvironmentConnection()).bind_to_dropdown_list(ddlGender, select_value: "0");
                PCentralCodeLookup.name_prefixes(Utilities.GetEnvironmentConnection()).bind_to_dropdown_list(ddlPrefix, select_value: "0");
                PCentralCodeLookup.name_suffixes(Utilities.GetEnvironmentConnection()).bind_to_dropdown_list(ddlSuffix, select_value: "0");
            }
        }

        protected void btnGetMatches_Click(object sender, EventArgs e)
        {
            PCentralPerson P = new PCentralPerson(Utilities.GetEnvironmentConnection())
            {
                FirstName = txtLegalFirstName.Text,
                LastName = txtLegalLastName.Text,
                MiddleName = txtLegalMiddleName.Text,
                Prefix = ddlPrefix.SelectedValue.toEnumNullable<Enumerations.enumNamePrefixes>(),
                Suffix = ddlPrefix.SelectedValue.toEnumNullable<Enumerations.enumNameSuffixes>(),
                GenderID = ddlPrefix.SelectedValue.toEnumNullable<Enumerations.enumISOGenderCodes>() ?? default(Enumerations.enumISOGenderCodes),
                OrganizationUniqueNbr = txtClientProvidedID.Text,
                BirthDate = txtBirthDate.Text.toDateTimeNullable()
            };

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                P.add_email_address(new PCentralEmailAddress(Utilities.GetEnvironmentConnection()) { Email = txtEmail.Text });
            }

            DataSet ds = PCentralPerson.ReadPersonMatches(P, ContextManager.ProgramID);

            if (ds.HasRows())
            {
                litMatches.Text = DB_Extensions.toHTML(ds.Tables[0], "class=\"CSSTableGenerator\"");
            }
            else
            {
                litMatches.Text = "<strong>No Matches Found</strong>";
            }
        }
    }
}