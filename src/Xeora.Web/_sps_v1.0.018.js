﻿function XeoraJS(){this.bindId="_sys_bind_",this.httprequests=new Array}XeoraJS.prototype.pushCode=function(t){this.bindId=this.bindId.substring(0,10),this.bindId+=new String(t)},XeoraJS.prototype.post=function(t){document.getElementById(this.bindId).value=t,document.forms[0].submit()},XeoraJS.prototype.createHttpRequest=function(){var t=null;if(window.XMLHttpRequest)try{t=new XMLHttpRequest}catch(e){t=null}if(null==t&&window.ActiveXObject)try{t=new ActiveXObject("Msxml2.XMLHTTP")}catch(e){try{t=new ActiveXObject("Microsoft.XMLHTTP")}catch(e){t=null}}return t},XeoraJS.prototype.update=function(t,e,n){null!=e&&"function"==typeof e&&(n=e,e=null);var r=this.httprequests.length;if(this.httprequests[r]=this.createHttpRequest(),null!=this.httprequests[r]){var i=(t=new String(t)).split(","),s=new String(i[0]),o=t.slice(s.length+1,t.length),l=null,u=s.split(">");if(u.length>0&&(l=document.getElementById(u[0])),null!=l){for(var c=1;c<u.length;c++){var d=this.findObjectById(l,u[c],1);null!=d&&(l=d)}this.httprequests[r].onreadystatechange=function(){__XeoraJS.processstate(l.id,o,e,n,r)},this.httprequests[r].open("POST",document.forms[0].action,!0),this.httprequests[r].setRequestHeader("Content-Type","application/x-www-form-urlencoded; charset=UTF-8"),this.httprequests[r].setRequestHeader("X-BlockRenderingID",s);var a="";null!=e&&""!=e&&(a=this.bindId+"="+e+"&");for(var h=0;h<document.forms[0].length;h++)document.forms[0][h].name!=this.bindId&&("checkbox"==document.forms[0][h].type.toLowerCase()||"radio"==document.forms[0][h].type.toLowerCase()?document.forms[0][h].checked&&(a+="&"+document.forms[0][h].name+"="+encodeURIComponent(document.forms[0][h].value)):a+="&"+document.forms[0][h].name+"="+encodeURIComponent(document.forms[0][h].value));if(null==n){var p=this.findObjectById(l,"indicator",null);null!=p&&(p.style.display="")}else n(0);this.httprequests[r].send(a)}else this.update(o,e)}else this.post(e)},XeoraJS.prototype.processstate=function(divID,nextDivIDs,assemblyInfo,indicatorCallback,httprequestindex){if(4==this.httprequests[httprequestindex].readyState){var continueOperation=!1;if(200==this.httprequests[httprequestindex].status){var rText=new String(this.httprequests[httprequestindex].responseText);if(0==rText.indexOf("rl:",0))return void(document.location.href=rText.substring(3,rText.length));continueOperation=!0;var resultSource=document.createElement("SPAN");resultSource.innerHTML=rText;var evalScript=this.compileScriptTags(resultSource);document.getElementById(divID).replaceWith(resultSource.firstChild),eval(evalScript),null!=indicatorCallback&&indicatorCallback(1)}else null!=indicatorCallback&&indicatorCallback(-1);this.httprequests[httprequestindex]=null,continueOperation&&null!=nextDivIDs&&""!=nextDivIDs&&this.update(nextDivIDs,assemblyInfo)}},XeoraJS.prototype.findObjectById=function(t,e,n){if(n<0)return null;if(t.id==e)return t;if(!t.hasChildNodes())return null;for(var r=0;r<t.childNodes.length;r++)if(returnObject=this.findObjectById(t.childNodes[r],e,null==n?null:n-1),null!=returnObject)return returnObject;return null},XeoraJS.prototype.compileScriptTags=function(t){for(var e="",n=0;n<t.childNodes.length;n++)t.childNodes[n].childNodes.length>0&&(e+=this.compileScriptTags(t.childNodes[n])),"SCRIPT"==t.childNodes[n].tagName&&(e+=t.childNodes[n].innerHTML);return e};var __XeoraJS=new XeoraJS;