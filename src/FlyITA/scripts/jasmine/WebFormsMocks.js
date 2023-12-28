function SubmitForm() {
    return true;
}

var mockObject = new Object();
mockObject.add_endRequest = function(thing) {};

var Sys = new Object();
var webforms = new Object();
var requestManager = new Object();
requestManager.getInstance = function () { return mockObject; };
Sys.WebForms = webforms;
Sys.WebForms.PageRequestManager = requestManager;