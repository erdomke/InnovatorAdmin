
/** ..\BrowserCode\common\javascript\XmlHttpRequestManagerCommon.js **/
function XmlRequestImplementation() {
	this.xhr = new XMLHttpRequest();
	this.headers = {};
}

XmlRequestImplementation.prototype.open = function (method, url, isAsync) {
	isAsync = (isAsync === 1 || isAsync === true);
	this.xhr.open(method, url, isAsync);
};

Object.defineProperty(XmlRequestImplementation.prototype, "onreadystatechange", {
	set: function (value) {
		this.xhr.onreadystatechange = value;
	}
});

XmlRequestImplementation.prototype.setRequestHeader = function (name, value) {
	this.headers[name] = value;
};

XmlRequestImplementation.prototype.send = function (body) {
	for (var key in this.headers) {
		if (this.headers.hasOwnProperty(key)) {
			this.xhr.setRequestHeader(key, this.headers[key]);
		}
	}
	//this line is required for IE 10.
	try { this.xhr.responseType = "msxml-document"; } catch (e) { }
	this.xhr.send(body);
};

XmlRequestImplementation.prototype.getResponseHeader = function (header) {
	return this.xhr.getResponseHeader(header);
};

XmlRequestImplementation.prototype.getAllResponseHeaders = function () {
	return this.xhr.getAllResponseHeaders();
};

Object.defineProperty(XmlRequestImplementation.prototype, "readyState", {
	get: function () {
		return this.xhr.readyState;
	}
});

Object.defineProperty(XmlRequestImplementation.prototype, "status", {
	get: function () {
		return this.xhr.status;
	}
});

Object.defineProperty(XmlRequestImplementation.prototype, "responseText", {
	get: function () {
		return this.xhr.responseText;
	}
});

Object.defineProperty(XmlRequestImplementation.prototype, "responseXML", {
	get: function () {
		return this.xhr.responseXML;
	}
});

function XmlHttpRequestManagerCommon() {
}

XmlHttpRequestManagerCommon.prototype.CreateRequest = function () {
	return new XmlRequestImplementation();
};
/** ..\BrowserCode\ff\javascript\XmlHttpRequestManager.js **/
function XmlHttpRequestManager() {
	XmlHttpRequestManagerCommon.call(this); //jshint ignore:line
}
XmlHttpRequestManager.prototype = XmlHttpRequestManagerCommon.prototype; //jshint ignore:line
/** ..\BrowserCode\common\javascript\XmlDocumentCommon.js **/
function XmlDocumentCommon() {
	var doc = document.implementation.createDocument("", "", null);
	doc.async = false;
	doc.preserveWhiteSpace = true;
	return doc;
}

Document.prototype.loadXML = function XmlDocumentLoadXML(xmlString) {
	if (!xmlString || typeof (xmlString) !== "string") {
		return false;
	}
	xmlString = xmlString.replace(/xmlns:i18n="http:\/\/www.w3.org\/XML\/1998\/namespace"/g, "xmlns:i18n=\"http://www.aras.com/I18N\"");
	var parser = new DOMParser();
	var doc = parser.parseFromString(xmlString, "text/xml");
	if (!this.documentElement) {
		this.appendChild(this.createElement("oldTeg"));
	}
	this.replaceChild(doc.documentElement, this.documentElement);
	return (this.parseError.errorCode === 0);
};

Document.prototype.loadUrl = function XmlDocumentLoadUrl(filename) {
	if (!filename || typeof (filename) !== "string") {
		return;
	}
	if (window.ActiveXObject) {
	  xhttp = new ActiveXObject("Msxml2.XMLHTTP");
	  }
	else {
	  xhttp = new XMLHttpRequest();
	  }
	xhttp.open("GET", filename, false);
	try { xhttp.responseType = "msxml-document"; } catch(err) {} // Helping IE11
	xhttp.send("");
	this.loadXML(xhttp.response);
};

Document.prototype.selectSingleNode = function XmlDocumentSelectSingleNode(xPath) {
	var xpe = new XPathEvaluator();
	var ownerDoc = this.ownerDocument == null ? this.documentElement : this.ownerDocument.documentElement;
	var nsResolver = xpe.createNSResolver(ownerDoc == null ? this : ownerDoc);

	return xpe.evaluate(xPath, this, nsResolver, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
};
Element.prototype.selectSingleNode = Document.prototype.selectSingleNode;

Document.prototype.selectNodes = function XmlDocumentSelectNodes(xPath) {
	var xpe = new XPathEvaluator();
	var ownerDoc = this.ownerDocument == null ? this.documentElement : this.ownerDocument.documentElement;
	var nsResolver = xpe.createNSResolver(ownerDoc == null ? this : ownerDoc);

	var result = xpe.evaluate(xPath, this, nsResolver, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
	var res = null;
	if (result) {
		res = [];
		for (var i = 0; i < result.snapshotLength; i++) {
			res.push(result.snapshotItem(i));
		}
	}
	return res;
};
Element.prototype.selectNodes = Document.prototype.selectNodes;

Element.prototype.transformNode = function XmlElementTransformNode(xmlDoc)
{
	var outputNode = xmlDoc.selectSingleNode("./xsl:stylesheet/xsl:output");
	var isHtml = (outputNode && outputNode.getAttribute("method") === "html");
	var sourceDoc = document.implementation.createDocument("", "", null);
	var node = sourceDoc.importNode(this, true);
	sourceDoc.appendChild(node);
	var processor = new XSLTProcessor();
	processor.importStylesheet(xmlDoc);
	var resultDoc = processor.transformToDocument(sourceDoc);
	return isHtml ? resultDoc.documentElement.outerHTML : resultDoc.xml;
};

Document.prototype.transformNode = function XmlDocumentTransformNode(xmlDoc) {
	var processor = new XSLTProcessor();
	processor.importStylesheet(xmlDoc);
	var outputNode = xmlDoc.selectSingleNode("./xsl:stylesheet/xsl:output");
	var isHtml = (outputNode && outputNode.getAttribute("method") === "html");
	var resultDoc = processor.transformToDocument(this);
	return isHtml ? resultDoc.documentElement.outerHTML : resultDoc.xml;
};

Document.prototype.createNode = function XmlDocumentTransformNode(nodeType, name, namespaceUrl) {
	var newNode = this.createElementNS(namespaceUrl, name);
	return newNode;
};

if (Document.prototype.__defineGetter__) {
	Document.prototype.__defineGetter__("text", function () {
		return this.textContent;
	});

	Document.prototype.__defineSetter__("text", function (value) {
		this.textContent = value;
	});

	Element.prototype.__defineGetter__("text", function () {
		return this.textContent;
	});

	Element.prototype.__defineSetter__("text", function (value) {
		this.textContent = value;
	});

	Text.prototype.__defineGetter__("text", function () {
		return this.textContent;
	});

	Attr.prototype.__defineGetter__("text", function (value) {
		return this.textContent;
	});

	Document.prototype.__defineGetter__("xml", function () {
		return (new XMLSerializer()).serializeToString(this);
	});

	Element.prototype.__defineGetter__("xml", function () {
		return (new XMLSerializer()).serializeToString(this);
	});

	Document.prototype.__defineGetter__("parseError", function () {
		if (!this.documentElement) {
			return { errorCode: 0 };
		}

		var parse = {};

		if (this.documentElement.nodeName == "parsererror") {
			parse.errorCode = 1;
			parse.srcText = "";
			parse.reason = this.documentElement.childNodes[0].nodeValue;
			return parse;
		}

		parse.errorCode = 0;
		return parse;
	});
}
/** ..\BrowserCode\ff\javascript\XmlDocument.js **/
function XmlDocument() {
	return new XmlDocumentCommon();
}

XMLDocument.prototype.load = function Document_load(docUrl) {
	var xmlhttp = new XmlHttpRequestManager().CreateRequest();
	xmlhttp.open("GET", docUrl, false);
	xmlhttp.send(null);
	this.loadXML(xmlhttp.responseText);
};