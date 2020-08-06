$(document).ready(function () {

    $('select#ddlPageSize').on('change', function () {

        $('select#ddlPaging').val(1)
        var pagenumber = $('select#ddlPaging').val();
        loadrecordlist(pagenumber);
    });

    $('select#ddlPaging').on('change', function () {

        var pagenumber = $('select#ddlPaging').val();
        loadrecordlist(pagenumber);

    });

    $('#divpagination a.previous').on('click', function () {

        var pagenumber = parseInt($('select#ddlPaging').val());
        pagenumber = pagenumber - 1;
        if (pagenumber >= 1) {
            loadrecordlist(pagenumber);
        }

    });

    $('#divpagination a.next').on('click', function () {

        var pagenumber = parseInt($('select#ddlPaging').val());
        var lastpageno = parseInt($('select#ddlPaging option:last-child').val());
        pagenumber = pagenumber + 1;
        if (lastpageno >= pagenumber) {
            loadrecordlist(pagenumber);
        }

    });

    $('#divpagination a.first').on('click', function () {

        var pagenumber = parseInt($('select#ddlPaging').val());
        if (pagenumber > 1) {
            pagenumber = 1;
            loadrecordlist(pagenumber);
        }
    });

    $('#divpagination a.last').on('click', function () {
        var pagenumber = parseInt($('select#ddlPaging option:last-child').val());
        if (pagenumber > 1) {
            loadrecordlist(pagenumber);
        }
    });

    $('table.tbl-record-list input.active').on('switchChange.bootstrapSwitch', function (event, state) {

        var cofirmmessage = 'Are you sure you want to inactive this?';
        var successmessage = 'Data set to inactive successfully.';

        if (state) {

            cofirmmessage = 'Are you sure you want to active this?';
            successmessage = 'Data set to active successfully.';
        }

        $(this).bootstrapSwitch('toggleState', true);

        var $element = $(this);

        customConfirm({ message: cofirmmessage },
            function (retunVal) {

                var loadurl = $element.attr('data-action-url');

                $('div#loaderdiv').show();

                $.ajax({
                    url: loadurl,
                    type: 'POST',
                    success: function (result) {
                        if (result == 'success') {
                            $('div#loaderdiv').hide();
                            loadrecordlist(1);
                            showalertpopup(successmessage);
                        }
                        else {
                            $('div#loaderdiv').hide();
                            $("div.failure-message").html('This Class is Full');
                        }

                    }
                });
            });

    });

    $('table.tbl-record-list input.deleted').on('switchChange.bootstrapSwitch', function (event, state) {

        var cofirmmessage = 'Are you sure you want to undo this delete?';
        var successmessage = 'Data delete undo successfully.';

        if (state) {

            cofirmmessage = 'Are you sure you want to delete this?';
            successmessage = 'Data deleted successfully.';
        }

        $(this).bootstrapSwitch('toggleState', true);

        var $element = $(this);

        customConfirm({ message: cofirmmessage },
            function (retunVal) {

                var loadurl = $element.attr('data-action-url');

                $('div#loaderdiv').show();

                $.ajax({
                    url: loadurl,
                    type: 'POST',
                    success: function (result) {
                        if (result == 'success') {
                            $('div#loaderdiv').hide();
                            loadrecordlist(1);
                            showalertpopup(successmessage);
                        }
                        else {
                            $('div#loaderdiv').hide();
                            $("div.failure-message").html('This Class is Full');
                        }

                    }
                });
            },
            function (retunVal) {
                if (state) {
                    showalertpopup('No data was deleted.');
                }
            });

    });

    $('table.tbl-record-list a.physicaldelete').on('click', function () {
        var cofirmmessage = 'Are you sure you want to delete this?';
        var successmessage = 'Data deleted successfully.';

        var $element = $(this);

        customConfirm({ message: cofirmmessage },
            function (retunVal) {

                var loadurl = $element.attr('data-action-url');

                $('div#loaderdiv').show();

                $.ajax({
                    url: loadurl,
                    type: 'POST',
                    success: function (result) {
                        if (result == 'success') {
                            $('div#loaderdiv').hide();
                            loadrecordlist(1);
                            showalertpopup(successmessage);
                        }
                        else if (result == 'failed') {
                            $('div#loaderdiv').hide();
                            showalertpopup('Failed to delete');
                        }
                        else {
                            $('div#loaderdiv').hide();
                            showalertpopup(result, 10000);
                        }
                    }
                });
            },
            function (retunVal) {
                showalertpopup('No data was deleted.');
            });

    });

    $('table.tbl-record-list a.cancel').on('click', function () {
        var cofirmmessage = 'Are you sure you want to cancel this?';
        var successmessage = 'Data cancelled successfully.';

        var $element = $(this);

        customConfirm({ message: cofirmmessage },
            function (retunVal) {

                var loadurl = $element.attr('data-action-url');

                $('div#loaderdiv').show();

                $.ajax({
                    url: loadurl,
                    type: 'POST',
                    success: function (result) {
                        if (result == 'success') {
                            $('div#loaderdiv').hide();
                            loadrecordlist(1);
                            showalertpopup(successmessage);
                        }
                        else if (result == 'failed') {
                            $('div#loaderdiv').hide();
                            showalertpopup('Failed to cancel');
                        }
                        else {
                            $('div#loaderdiv').hide();
                            showalertpopup(result, 10000);
                        }
                    }
                });
            },
            function (retunVal) {
                showalertpopup('No data was cancelled.');
            });

    });

});



