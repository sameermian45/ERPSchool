
$(function () {
    // Initialize form validation on the registration form.
    // It has the name attribute "registration"
    $("form[name='EmployeeForm']").validate({

        rules: {
            FirstName: "required",
            LastName: "required",
            email: {
                required: true,
                // Specify that email should be validated
                // by the built-in "email" rule
                email: true
            },
            password: {
                required: true,
                minlength: 5
            }
        },
        // Specify validation error messages
        messages: {
            firstname: "Please enter your firstname",
            lastname: "Please enter your lastname",
            password: {
                required: "Please provide a password",
                minlength: "Your password must be at least 5 characters long"
            },
            email: "Please enter a valid email address"
        },
     
        submitHandler: function (form) {
            form.submit();
        }
    });
});