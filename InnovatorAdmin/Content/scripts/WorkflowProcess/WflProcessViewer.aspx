<!DOCTYPE html>
<!-- (c) Copyright by Aras Corporation, 2004-2013. -->
<html>
<!--
////////////////////////////////////////////////////////////////////////////////////////
//    File: WflProcessViewer.ASPx
//    Date Create:   03/13/03  peter schroer
//                   11/17/03  phs   show assignees using rt mouse menu
//                   11/18/03  phs   skip cloned nodes
//    opens a workflow instance for graphic viewing
//
///////////////////////////////////////////////////////////////////////////////////////
// end of comment -->
<head>
	<meta http-equiv="Pragma" content="no-cache">
	<script language="JavaScript">
var aras = null;
var processID = <% Response.Write("'" & Request.QueryString("pid") & "'") %>;
var activityID = "";
var processName = "";

<%
Dim pid: pid = Request.QueryString("pid")
If IsNothing(pid) Then pid = ""

If pid = "" Then%>
	aras = parent.dialogArguments ? parent.dialogArguments.aras : dialogArguments.aras;
	processID = parent.dialogArguments ? parent.dialogArguments.processID : dialogArguments.processID;
	activityID = parent.dialogArguments ? parent.dialogArguments.activityID : dialogArguments.activityID;
	processName = parent.dialogArguments ? parent.dialogArguments.processName : dialogArguments.processName;
	<%Else%>
	aras = parent.parent.aras;
<%End If%>
	aras.browserHelper.hidePanels(window, ["locationbar"]);
	</script>
	<script type="text/javascript" src="../../javascript/PopulateDocByLabels.js"></script>
	<style type="text/css">
		@import "../../javascript/dojo/resources/dojo.css";
		@import "../../javascript/dijit/themes/claro/claro.css";

		html, body {
			overflow: hidden;
			width: 100%;
			height: 100%;
			margin: 0px;
			padding: 0px;
		}
	</style>
	<script type="text/javascript" src="../../javascript/include.aspx?classes=/dojo.js" data-dojo-config="baseUrl:'../../javascript/dojo'"></script>
	<script type="text/javascript" src="../../javascript/include.aspx?classes=XmlDocument"></script>
<script type="text/javascript">
	var workflowApplet = null,
		toolbar = null;

	PopulateDocByLabels();
	clientControlsFactory.createControl("Aras.Client.Controls.Experimental.Workflow", "workflow", function(control) {
		workflowApplet = control;
		workflowInit();
	});

	clientControlsFactory.createControl("Aras.Client.Controls.Public.ToolBar", { id: "wflprocessviewer_toolbar", connectId: "toolbar" }, function(control) {
		toolbar = control;
		clientControlsFactory.on(toolbar, {
			"onClick": toolbarClick_handler
		});
		toolbarLoadHandler();
	});

	function workflowInit() {
		var WflXml = aras.loadProcessInstance(processID, activityID);
		if (!WflXml) {
			aras.AlertError(aras.getResource("", "wfiprocessviewer.failed_load_wp_instance"));
			return;
		}
		workflowApplet.load(WflXml.xml);
	}

	function toolbarLoadHandler()
	{
		document.toolbar = toolbar;
		toolbar.loadXml(aras.getI18NXMLResource("wflprocessviewer_toolbar.xml"));
		toolbar.show();

		if (!processID || processID == "") {
			toolbar.getItem("view_signoffs").disable();
		}
		toolbar.getItem("workflow_process").setText(processName);
	}

	function viewSignoffsClickHandler() {
		var ProcessItem = aras.getItemById("Workflow Process", processID);
		if (!ProcessItem) return;
		var report = aras.getItemByKeyedName("Report", "Workflow Process History");
		var processType = aras.getItemTypeForClient("Workflow Process", "name").node;
		var processTypeID = processType.getAttribute("id");
		aras.runReport(report, processTypeID, ProcessItem);
	}

	function toolbarClick_handler(item) {
		var tb = toolbar.getActiveToolbar();
		tb.disable();
		if(item.getId() == "view_signoffs") {
			viewSignoffsClickHandler();
		}
		tb.enable();
	}
</script>
</head>
<body class="claro">
	<div id="toolbar" style="position: fixed; top: 0px; height:30px; width: 100%;"></div>
	<div id="workflow" style="position: fixed; top: 30px; bottom:0px; width: 100%; overflow:auto;"></div>
</body>
</html>
