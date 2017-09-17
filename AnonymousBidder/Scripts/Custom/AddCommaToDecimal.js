function ReplaceNumberWithCommas(yourNumber) {
    //Seperates the components of the number
    var components = yourNumber.toString().split(".");
    //Comma-fies the first part
    components[0] = components[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    //Combines the two sections
    return components.join(".");
}
$('#add-item').on('shown.bs.modal', function (e) {
    $(".numberWithCommas").each(function () {
        var text = $(this).text();
        $(this).html(ReplaceNumberWithCommas(text.trim()));
    });
})
$(document).ready(function () {
    $(".numberWithCommas").each(function () {
        var text = $(this).text();
        $(this).html(ReplaceNumberWithCommas(text.trim()));
    });
});
