﻿function XeoraJS() {
    this.bindId = "_sys_bind_";
    this.httprequests = new Array();
};

XeoraJS.prototype.pushCode = function (callCode) {
    this.bindId += new String(callCode);
};

XeoraJS.prototype.post = function (AssemblyInfo) {
    document.getElementById(this.bindId).value = AssemblyInfo;
    document.forms[0].submit();
};

XeoraJS.prototype.createHttpRequest = function () {
    var httprequest = null;

    if (window.XMLHttpRequest) {
        try { httprequest = new XMLHttpRequest(); }
        catch (e) { httprequest = null; }
    }

    if (httprequest == null && window.ActiveXObject) {
        try { httprequest = new ActiveXObject("Msxml2.XMLHTTP"); }
        catch (e) {
            try { httprequest = new ActiveXObject("Microsoft.XMLHTTP"); }
            catch (e) { httprequest = null; }
        }
    }

    return httprequest;
};

XeoraJS.prototype.update = function (UpdateLocation, AssemblyInfo) {
    var httprequestIndex = this.httprequests.length;

    this.httprequests[httprequestIndex] = this.createHttpRequest();

    if (this.httprequests[httprequestIndex] != null) {
        UpdateLocation = new String(UpdateLocation);

        var allUpdateLocations = UpdateLocation.split(",");
        var currentUpdateLocation = new String(allUpdateLocations[0]);
        var nextUpdateLocations = UpdateLocation.slice(currentUpdateLocation.length + 1, UpdateLocation.length);

        if (document.getElementById(currentUpdateLocation) != null) {
            this.httprequests[httprequestIndex].onreadystatechange = function () { __XeoraJS.processstate(currentUpdateLocation, nextUpdateLocations, AssemblyInfo, httprequestIndex); };
            this.httprequests[httprequestIndex].open("POST", document.forms[0].action, true);
            this.httprequests[httprequestIndex].setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
            this.httprequests[httprequestIndex].setRequestHeader('X-BlockRenderingID', currentUpdateLocation);

            var postContent = "";

            if (AssemblyInfo != null && AssemblyInfo != "")
            { postContent = this.bindId + "=" + AssemblyInfo + "&"; }

            for (var iC = 0; iC < document.forms[0].length; iC++) {
                if (document.forms[0][iC].name != this.bindId) {
                    if (document.forms[0][iC].type.toLowerCase() == "checkbox" || document.forms[0][iC].type.toLowerCase() == "radio") {
                        if (document.forms[0][iC].checked)
                        { postContent += "&" + document.forms[0][iC].name + "=" + encodeURIComponent(document.forms[0][iC].value); }
                    }
                    else
                    { postContent += "&" + document.forms[0][iC].name + "=" + encodeURIComponent(document.forms[0][iC].value); }
                }
            }

            var indicatorObject = this.findObjectById(document.getElementById(currentUpdateLocation), "indicator");

            if (indicatorObject != null)
            { indicatorObject.style.display = ""; }

            this.httprequests[httprequestIndex].send(postContent);
        }
        else
        { this.update(nextUpdateLocations, AssemblyInfo); }
    }
    else
    { this.post(AssemblyInfo); }
};

XeoraJS.prototype.processstate = function (divID, nextDivIDs, AssemblyInfo, httprequestindex) {
    if (this.httprequests[httprequestindex].readyState == 4) {
        var continueOperation = false;

        if (this.httprequests[httprequestindex].status == 200) {
            var rText = new String(this.httprequests[httprequestindex].responseText);

            if (rText.indexOf("rl:", 0) == 0)
            { document.location.href = rText.substring(3, rText.length); }
            else {
                document.getElementById(divID).innerHTML = rText;
                continueOperation = true;

                var resultSource = document.createElement("SPAN");
                resultSource.innerHTML = rText;

                var evalScript = this.compileScriptTags(resultSource);
                eval(evalScript);
            }
        }
        else {
            var errorObject = this.findObjectById(document.getElementById(divID), "error");

            if (errorObject != null)
            { errorObject.style.display = ""; }
            else
            { document.getElementById(divID).innerHTML = ""; }
        }

        this.httprequests[httprequestindex] = null;

        if (continueOperation && nextDivIDs != null && nextDivIDs != "")
        { this.update(nextDivIDs, AssemblyInfo); }
    }
};

XeoraJS.prototype.findObjectById = function (searchObject, searchID) {
    var returnObject = null;

    if (searchObject.hasChildNodes()) {
        for (var cC = 0; cC < searchObject.childNodes.length; cC++) {
            if (searchObject.childNodes[cC].id == searchID)
            { returnObject = searchObject.childNodes[cC]; break; }

            if (searchObject.childNodes[cC].hasChildNodes()) {
                returnObject = this.findObjectById(searchObject.childNodes[cC], searchID);

                if (returnObject != null) { break; }
            }
        }
    }

    return returnObject;
};

XeoraJS.prototype.compileScriptTags = function (element) {
    var returnValue = "";

    for (var cX = 0; cX < element.childNodes.length; cX++) {
        if (element.childNodes[cX].childNodes.length > 0)
        { returnValue += this.compileScriptTags(element.childNodes[cX]); }

        if (element.childNodes[cX].tagName == "SCRIPT")
        { returnValue += element.childNodes[cX].innerHTML; }
    }

    return returnValue;
};

var __XeoraJS = new XeoraJS();