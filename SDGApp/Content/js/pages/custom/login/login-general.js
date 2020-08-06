"use strict";

// Class Definition
var KTLoginGeneral = function () {

    var login = $('#kt_login');

    var showErrorMsg = function (form, type, msg) {
        var alert = $('<div class="alert alert-' + type + ' alert-dismissible" role="alert">\
			<div class="alert-text">'+ msg + '</div>\
			<div class="alert-close">\
                <i class="flaticon2-cross kt-icon-sm" data-dismiss="alert"></i>\
            </div>\
		</div>');

        form.find('.alert').remove();
        alert.prependTo(form);
        //alert.animateClass('fadeIn animated');
        KTUtil.animateClass(alert[0], 'fadeIn animated');
        alert.find('span').html(msg);
    };

    // Private Functions
    var displaySignUpForm = function () {

        login.removeClass('kt-login--forgot');
        login.removeClass('kt-login--signin');

        login.addClass('kt-login--signup');
        KTUtil.animateClass(login.find('.kt-login__signup')[0], 'flipInX animated');
    }

    var displaySignInForm = function () {
        login.removeClass('kt-login--forgot');
        login.removeClass('kt-login--signup');

        login.addClass('kt-login--signin');
        KTUtil.animateClass(login.find('.kt-login__signin')[0], 'flipInX animated');
        //login.find('.kt-login__signin').animateClass('flipInX animated');
    }

    var displayForgotForm = function () {
        login.removeClass('kt-login--signin');
        login.removeClass('kt-login--signup');

        login.addClass('kt-login--forgot');
        //login.find('.kt-login--forgot').animateClass('flipInX animated');
        KTUtil.animateClass(login.find('.kt-login__forgot')[0], 'flipInX animated');

    }

    var displayForgotUserForm = function () {
        login.removeClass('kt-login--signin');
        login.removeClass('kt-login--signup');

        login.addClass('kt-login--forgot');
        //login.find('.kt-login--forgot').animateClass('flipInX animated');
        KTUtil.animateClass(login.find('.kt-login__forgot')[1], 'flipInX animated');

    };

    var displayResetForm = function () {
        login.removeClass('kt-login--signin');
        login.removeClass('kt-login--signup');

        login.addClass('kt-login--reset');
        //login.find('.kt-login--forgot').animateClass('flipInX animated');
        KTUtil.animateClass(login.find('.kt-login__reset')[0], 'flipInX animated');

    }



    var handleFormSwitch = function () {

        $('#kt_login_forgot').click(function (e) {
            e.preventDefault();
            $('#dv-kt-login_forgot_username').hide();
            $('#dv-kt-login_forgot_password').show();
            displayForgotForm();
        });

        $('#kt_login_forgot_username').click(function (e) {
            e.preventDefault();

            $('#dv-kt-login_forgot_password').hide();
            $('#dv-kt-login_forgot_username').show();
            displayForgotUserForm();
        });

        $('#kt_login_forgot_cancel').click(function (e) {
            e.preventDefault();
            $('#dv-kt-login_forgot_password').hide();
            $('#dv-kt-login_forgot_username').hide();
            displaySignInForm();
        });

        $('#kt_login_forgot_username_cancel').click(function (e) {
            e.preventDefault();
            $('#dv-kt-login_forgot_password').hide();
            $('#dv-kt-login_forgot_username').hide();
            displaySignInForm();
        });

        $('#kt_login_signup').click(function (e) {

            $('#dv-kt-login_forgot_password').hide();
            $('#dv-kt-login_forgot_username').hide();
            $('#hG_signup').addClass('hG_Register');

            e.preventDefault();
            displaySignUpForm();
           

        });

        $('#kt_login_signup_cancel').click(function (e) {

            $('#hG_signup').removeClass('hG_Register');

            e.preventDefault();
            displaySignInForm();
        });

        $('#kt_login_reset').click(function (e) {
            e.preventDefault();
            displayResetForm();
        });

        $('#kt_login_reset_cancel').click(function (e) {
            e.preventDefault();
            displaySignInForm();
        });
    }



    var handleSignInFormSubmit = function () {
        $('#kt_login_signin_submit').click(function (e) {
            e.preventDefault();

            //var btn = $(this);
            //var form = $(this).closest('form');

            //form.validate({
            //    rules: {
            //        email: {
            //            required: true,
            //            email: true
            //        },
            //        password: {
            //            required: true
            //        }
            //    }
            //});

            //if (!form.valid()) {
            //    return;
            //}

            //btn.addClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', true);

            //form.ajaxSubmit({
            //    url: '',
            //    success: function(response, status, xhr, $form) {
            //    	// similate 2s delay
            //    	setTimeout(function() {
            //         btn.removeClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', false);
            //         showErrorMsg(form, 'danger', 'Incorrect username or password. Please try again.');
            //        }, 2000);
            //    }
            //});
        });
    }

    var handleSignUpFormSubmit = function () {

        $('#btnSignUp').click(function (e) {
            
            e.preventDefault();
            var btn = $(this);
            var form = $(this).closest('form');

            form.validate({
                rules: {
                    FirstName: {
                        required: true
                    },
                    LastName: {
                        required: true
                    },
                    UserName: {
                        required: true
                    },
                    Email: {
                        required: true,
                         email: true
                    },
                    Phone: {
                        required: true
                    },
                    NewPassword: {
                        required: true
                    },
                    ConfirmPassword: {
                        required: true
                    },
                    RememberMe: {
                        required: true
                    }
                }
            });

            if (!form.valid()) {
                $('#kt_modal_1_2').modal('show');
                return;
            } 

            //btn.addClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', true);
            form.submit();
           

        });

        $('#kt_login_signup_submit').click(function (e) {
            
            e.preventDefault();

            var btn = $(this);
            var form = $(this).closest('form');

            form.validate({
                rules: {
                    fullname: {
                        required: true
                    },
                    email: {
                        required: true,
                        email: true
                    },
                    password: {
                        required: true
                    },
                    rpassword: {
                        required: true
                    },
                    agree: {
                        required: true
                    }
                }
            });

            if (!form.valid()) {
                return;
            }

            btn.addClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', true);

            form.ajaxSubmit({
                url: '',
                success: function(response, status, xhr, $form) {
                	// similate 2s delay
                	setTimeout(function() {
                     btn.removeClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', false);
                     form.clearForm();
                     form.validate().resetForm();

                     // display signup form
                     displaySignInForm();
                     var signInForm = login.find('.kt-login__signin form');
                     signInForm.clearForm();
                     signInForm.validate().resetForm();

                     showErrorMsg(signInForm, 'success', 'Thank you. To complete your registration please check your email.');
                 }, 2000);
                }
            });
        });
    }

    var handleForgotFormSubmit = function () {
        $('#kt_login_forgot_submit').click(function (e) {
            e.preventDefault();

            var btn = $(this);
            var form = $(this).closest('form');

            form.validate({
                rules: {
                    email: {
                        required: true,
                        email: true
                    }
                }
            });

            if (!form.valid()) {
                return;
            }

            btn.addClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', true);

            form.ajaxSubmit({
                url: '',
                success: function (response, status, xhr, $form) {
                    // similate 2s delay
                    setTimeout(function () {
                        btn.removeClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', false); // remove
                        form.clearForm(); // clear form
                        form.validate().resetForm(); // reset validation states

                        // display signup form
                        displaySignInForm();
                        var signInForm = login.find('.kt-login__signin form');
                        signInForm.clearForm();
                        signInForm.validate().resetForm();

                        showErrorMsg(signInForm, 'success', 'Cool! Password recovery instruction has been sent to your email.');
                    }, 2000);
                }
            });
        });
    }

    var handleResetFormSubmit = function () {
        $('#kt_login_reset_submit').click(function (e) {
            e.preventDefault();

            var btn = $(this);
            var form = $(this).closest('form');

            form.validate({
                rules: {
                    password: {
                        required: true
                    },
                    rpassword: {
                        required: true
                    },
                }
            });

            if (!form.valid()) {
                return;
            }

            btn.addClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', true);

            form.ajaxSubmit({
                url: '',
                success: function (response, status, xhr, $form) {
                    // similate 2s delay
                    setTimeout(function () {
                        btn.removeClass('kt-spinner kt-spinner--right kt-spinner--sm kt-spinner--light').attr('disabled', false); // remove
                        form.clearForm(); // clear form
                        form.validate().resetForm(); // reset validation states

                        // display signup form
                        displaySignInForm();
                        var signInForm = login.find('.kt-login__signin form');
                        signInForm.clearForm();
                        signInForm.validate().resetForm();

                        showErrorMsg(signInForm, 'success', 'Cool! Reset Password recovery instruction has been sent to your email.');
                    }, 2000);
                }
            });
        });
    }



    // Public Functions
    return {
        // public functions
        init: function () {
            handleFormSwitch();
            handleSignInFormSubmit();
            handleSignUpFormSubmit();
            handleForgotFormSubmit();
            handleResetFormSubmit();
        }
    };
}();

// Class Initialization
jQuery(document).ready(function () {
    KTLoginGeneral.init();


});

