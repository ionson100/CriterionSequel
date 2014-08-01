
function viewHelp(id) {
     $.get("/ss/help/text/" + id, function(data) {
         $("#dialog").empty();
         $("#dialog").append(data);
         $("#dialog").dialog("open");
     });
};
