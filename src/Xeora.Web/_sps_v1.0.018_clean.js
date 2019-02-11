function XeoraJS() {
    this.bindId = "_sys_bind_";
    this.httprequests = new Array();
};

XeoraJS.prototype.pushCode = function (callCode) {
    this.bindId = this.bindId.substring(0, 10);
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

XeoraJS.prototype.update = function (updateLocation, assemblyInfo, indicatorCallback) {
    if (assemblyInfo != null && typeof assemblyInfo == "function") {
        indicatorCallback = assemblyInfo;
        assemblyInfo = null;
    }

    var httprequestIndex = this.httprequests.length;

    this.httprequests[httprequestIndex] = this.createHttpRequest();

    if (this.httprequests[httprequestIndex] == null) {
        this.post(assemblyInfo); 
        return;
    }

    updateLocation = new String(updateLocation);

    var allUpdateLocations = updateLocation.split(",");
    var currentUpdateLocation = new String(allUpdateLocations[0]);
    var nextUpdateLocations = updateLocation.slice(currentUpdateLocation.length + 1, updateLocation.length);

    var mostAvailableObject = null;
    var pathOfUpdateLocation = currentUpdateLocation.split(">");
    if (pathOfUpdateLocation.length > 0) {
        mostAvailableObject = document.getElementById(pathOfUpdateLocation[0]);
    }

    if (mostAvailableObject == null) {
        this.update(nextUpdateLocations, assemblyInfo);
        return;
    }

    for (var i = 1; i < pathOfUpdateLocation.length; i++) {
        var innerObject = this.findObjectById(mostAvailableObject, pathOfUpdateLocation[i], 1);
        if (innerObject != null) {
            mostAvailableObject = innerObject;
        }
    }

    this.httprequests[httprequestIndex].onreadystatechange = function () { __XeoraJS.processstate(mostAvailableObject.id, nextUpdateLocations, assemblyInfo, indicatorCallback, httprequestIndex); };
    this.httprequests[httprequestIndex].open("POST", document.forms[0].action, true);
    this.httprequests[httprequestIndex].setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
    this.httprequests[httprequestIndex].setRequestHeader('X-BlockRenderingID', currentUpdateLocation);

    var postContent = "";
    if (assemblyInfo != null && assemblyInfo != "")
    { postContent = this.bindId + "=" + assemblyInfo + "&"; }

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

    if (indicatorCallback == null) {
        var indicatorObject = this.findObjectById(mostAvailableObject, "indicator", null);
        if (indicatorObject != null) {
            indicatorObject.style.display = "";
        }
    } else {
        indicatorCallback(0);
    }

    this.httprequests[httprequestIndex].send(postContent);
};

XeoraJS.prototype.processstate = function (divID, nextDivIDs, assemblyInfo, indicatorCallback, httprequestindex) {
    if (this.httprequests[httprequestindex].readyState != 4) {
        return;
    }

    var continueOperation = false;

    if (this.httprequests[httprequestindex].status == 200) {
        var rText = new String(this.httprequests[httprequestindex].responseText);

        if (rText.indexOf("rl:", 0) == 0) {
            document.location.href = rText.substring(3, rText.length);
            return;
        }
        continueOperation = true;

        var resultSource = document.createElement("SPAN");
        resultSource.innerHTML = rText;

        var evalScript = this.compileScriptTags(resultSource);

        document.getElementById(divID).replaceWith(resultSource.firstChild);
        eval(evalScript);

        if (indicatorCallback != null) {
            indicatorCallback(1);
        }
    } else {
        if (indicatorCallback != null) {
            indicatorCallback(-1);
        }
    }

    this.httprequests[httprequestindex] = null;

    if (continueOperation && nextDivIDs != null && nextDivIDs != "")
    { this.update(nextDivIDs, assemblyInfo); }
};

XeoraJS.prototype.findObjectById = function (searchObject, searchID, depth) {
    if (depth < 0) {
        return null;
    }

    if (searchObject.id == searchID) {
        return searchObject;
    }

    if (!searchObject.hasChildNodes()) {
        return null;
    }

    for (var cC = 0; cC < searchObject.childNodes.length; cC++) {
        returnObject = this.findObjectById(searchObject.childNodes[cC], searchID, depth == null ? null : depth - 1);
        if (returnObject != null) { return returnObject }
    }

    return null;
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