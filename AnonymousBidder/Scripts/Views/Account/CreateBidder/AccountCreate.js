$(document).ready(function () {



    var validator = $("#registerBidderForm").kendoValidator({
        rules: {
        },
        messages: {
           
        }
    }).data("kendoValidator"),
    status = $(".status");



    $("#btnRegister").click(function (event) {
        event.preventDefault();
        if (!validator.validate()) {
            status.text("Oops! There is invalid data in the form.")
                .removeClass("valid")
                .addClass("invalid");
        }
        if (validator.validate()) {
            // Check if email is duplicate 
            $.ajax({
                type: "POST",
                url: '/Account/CheckBidderEmail',
                data: { emailAddress: $("#Bidder_Email").val() },
                success: function (result) {
                    if (result != true) {
                        alert("Email already in use.");
                    }
                    else {
                        var element = this;
                        $("#btnRegister").closest("form").submit();
                    }
                },
                error: function (xhr, status, p3, p4) {
                    alert("Couldn't complete function");
                }
            });
        }
    });


});