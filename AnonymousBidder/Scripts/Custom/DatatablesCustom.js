var fixedHeaders = [];
var dataTable;
function InitFixedHeaders(table, dTable) {
    dataTable = dTable;
   // fixedHeaders.push(new FixedHeader(table, {}));
    BindSearchEventToInput();
}

function BindSearchEventToInput(table) {
    $("table thead input").keyup(function () {
        //Filter on the column (the index) of this element
        var colIndex = $("table thead input").index(this);
        dataTable.column(colIndex + 1).search(this.value).draw();
        $("table thead input:eq(" + colIndex + ")").attr('value', this.value);
        $("#url").val(this.value);
    });
}
function updateFixedHeaders() {
    for (var i = 0; i < fixedHeaders.length; i++) {
        fixedHeaders[i]._fnUpdateClones(true); // force redraw
        fixedHeaders[i]._fnUpdatePositions();
    }
    $(".dataTables_wrapper ").append($(".FixedHeader_Cloned"));
    BindSearchEventToInput();
}

$(window).resize(function () {
    updateFixedHeaders();
});
$(document).ready(function () {
    //move the pagination of the table to the actions bar
    $(".table-pagination-top").ready(function () {
        $(".table-pagination-wrap").html($(".table-pagination-top"));
    });
});
