$(document).ready(function() {

    $("#SignUpNewsletter").on("submit",
        function () {
            var $this = $(this);
            var values = $this.serialize();
            $.ajax({
                type: $this.attr("method"),
                url: $this.attr("action"),
                data: values,
                success: function () {
                    //$("#info-boxContact").addClass("alert-success");
                    //$("#info-boxContact").removeClass("alert-danger");
                    //$("#info-boxContact").text("Tak for din tilmeldning");
                    //$("#info-boxContact").fadeIn().delay(500).fadeOut();
                    swal({
                        title: "Success?",
                        text: "Du har nu tilmeldt dig vores nyhedsbrev",
                        type: "success"

                    });

                    $this.trigger("reset");

                },
                error: function () {
                    swal({
                        title: "Error?",
                        text: "HOV.....noget gik galt- prøv igen",
                        type: "error"

                    });
                }
                //swal('Congratulations!', 'Your message has been successfully sent', 'success');

            });
            event.preventDefault();
        });




});