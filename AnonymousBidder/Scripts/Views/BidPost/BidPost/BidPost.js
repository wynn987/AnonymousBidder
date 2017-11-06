$(document).ready(function () {
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