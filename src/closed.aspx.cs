using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlyITA
{
    public partial class closed : System.Web.UI.Page, IClosedView
    {
        closedMVP _presenter;
        protected void Page_Load(object sender, EventArgs e)
        {
            _presenter = new closedMVP(this);

            if (!Page.IsPostBack)
            {
              //  //_Presenter.Init();
            }
        }
        public string ClosedMessage
        {
            set { lblClosedmsg.Text = value; }
        }

        public string CurrentPage
        {
            get { return Utilities.GetCurrentPage().Split('.')[0]; }
        }

        public System.Web.UI.WebControls.Label UserMessages
        {
            get { return lblClosedmsg; }
            set { lblClosedmsg.Text = value.Text; }
        }

    }
}