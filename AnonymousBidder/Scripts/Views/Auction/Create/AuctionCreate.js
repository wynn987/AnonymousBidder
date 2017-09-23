$(document).ready(function () {
    var startdate = $("#Auction_StartDate").kendoDateTimePicker({
        value: new Date(),
        format: 'dd/MM/yyyy HH:mm',
        timeFormat: 'HH:mm'
    });
    var enddate = $("#Auction_EndDate").kendoDateTimePicker({
        value: new Date(),
        format: 'dd/MM/yyyy HH:mm',
        timeFormat: 'HH:mm'
    });

    $("#files").kendoUpload({
        validation: {
            allowedExtensions: [".jpg", ".png"],
            maxFileSize: 4194304
        }
    });

    $("#Auction_StartingBid").kendoNumericTextBox();

    var validator = $("#AuctionCreateForm").kendoValidator({
        rules: {
            validDate: function (input) {
                if (input.is("[data-role=datetimepicker]")) {
                    if (startdate.val() && enddate.val()) {
                        return true;
                    }
                    return false;
                }
                return true;
            }, startDate: function (input) {
                if (input.is("[data-currentdate-field]") && input.val() != "") {
                    var date = kendo.parseDate(input.val());

                    return date.getDate() >= new Date().getDate();
                }

                return true;
            }, endDate: function (input) {
                if (input.is("[data-greaterdate-msg]") && input.val() != "") {
                    var date = kendo.parseDate(input.val()),
                        otherDate = kendo.parseDate($("[name='" + input.data("greaterdateField") + "']").val());
                    return otherDate == null || otherDate.getTime() < date.getTime();
                }

                return true;
            }

        },
        messages: {
            validDate: "Date should be in the format dd/MM/yyyy HH:mm: ",
            startDate: "Start date should be after or from today",
            endDate: "End date should be after Start date"
        }
    }).data("kendoValidator"),
        status = $(".status");

    $("#btnCreate").click(function (event) {
        event.preventDefault();
        if (!validator.validate()) {
            status.text("Oops! There is invalid data in the form.")
                .removeClass("valid")
                .addClass("invalid");
        }
        if (validator.validate()) {
            var element = this;
            $(element).closest("form").submit();
        }
    });
});