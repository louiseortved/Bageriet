$(document).ready(function () {
    $("#productCreate").bootstrapValidator({
        message: 'This value is not valid',
        feedbackIcons: {
            valid: 'icon-check',
            invalid: 'icon-remove',
            validating: 'icon-refresh'
        },
        row: {
            valid: 'field-success',
            invalid: 'field-error'
        },
   
    }
    );

    $("#productEdit").bootstrapValidator(
    );


    $("#sliderCreate").bootstrapValidator(
    );
    $("#sliderEdit").bootstrapValidator(
    );



    $("#sizeCreate").bootstrapValidator({
        message: 'The Title is required'
    });


    $("#sizeEdit").bootstrapValidator(
    );
   
    $("#newsletterCreate").bootstrapValidator(
    );
    $("#AboutEdit").bootstrapValidator(
    );
    $("#contentCreate").bootstrapValidator(
    );
   
    $("#BlogCreate").bootstrapValidator(
    );


    $("#categoryEdit").bootstrapValidator(
    );
    $("#categoryCreate").bootstrapValidator(
    );
    $("#brandEdit").bootstrapValidator(
    );
    $("#brandCreate").bootstrapValidator(
    );

    $("#editUser").bootstrapValidator(
    );
   


    $("#SignUpNews").bootstrapValidator(
    );


    $("#contactForm").bootstrapValidator(
    );

    
})