using ITALib;
using ITALib.DataAccess;
using System;
using System.Data;

namespace FlyITA
{
    public interface IClosedView
    {
        string CurrentPage { get; }
        string ClosedMessage { set; }
    }

    public class closedMVP : PresenterBase
    {
        #region " ### Constructor Code ### "
        IClosedView _view;

        public closedMVP(IClosedView myview)
        {
            _view = myview;
        }

        IClosedView View
        {
            get { return _view; }
            set { _view = value; }
        }
        #endregion

        #region " ### Presenter Methods ### "
        public void Init()
        {
            if (Utilities.ShouldReadWebRegSettings(View.CurrentPage))
            {
                SetUpPage();
            }
        }
        #endregion

        /// <summary>
        /// Reads Page setting from DB set up by user thru WebRegAdmin.
        /// Sets up page controls based on data from database.
        /// </summary>
        /// <remarks></remarks>
        public void SetUpPage()
        {
            PageConfiguration pageSettings = new PageConfiguration();
            DataSet settingsDS = pageSettings.ReadPageConfigSettings(View.CurrentPage);
            ClosedDTO settingsDTO = new ClosedDTO(settingsDS);

            //db settings
            if (settingsDS.HasTables())
            {
                //check if we need to show the page
                if (settingsDS.HasRows() && (!settingsDS.Tables[0].Rows[0]["ItemValue"].toBool()))
                {
                    //page was marked as SKIP, move to next page
                    Utilities.Navigate(Utilities.NextPage(), "Closed:skip");
                }

                //set up page here
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Closedmessage))
                {
                    View.ClosedMessage = settingsDTO.Closedmessage;
                }
            }
        }
    }

    #region " ## DTO ## "

    /// <summary>
    /// DTO class to set up DTO properties with values from db read.
    /// </summary>
    /// <remarks></remarks>
    public class ClosedDTO
    {
        /// <summary>
        /// Constructor used for user defined settings.
        /// </summary>
        /// <remarks></remarks>
        public ClosedDTO()
        {
            //'
        }
        /// <summary>
        /// Constructor used by read db settings method to populate DTO.
        /// </summary>
        /// <param name="settingsds"></param>
        /// <remarks></remarks>
        public ClosedDTO(DataSet settingsds)
        {
            if ((settingsds != null) && (settingsds.Tables[1] != null))
            {
                foreach (DataRow row in settingsds.Tables[1].Rows)
                {
                    //because this is not strongly typed, there is no point of using hashtable
                    //set up your properties so its easier to call it from the presenter
                    switch (row["ItemName"].ToString())
                    {
                        case "closeregistrationtext":
                            this._closedmessage = row["ItemValue"].ToString();

                            break;
                    }
                }
            }
        }
        private string _closedmessage;
        public string Closedmessage
        {
            get { return _closedmessage; }
            set { _closedmessage = value; }
        }

    }

    #endregion
}