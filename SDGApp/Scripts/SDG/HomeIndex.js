// select menu
selectleftmenu('dashboard', '');
var ArrDictionary = new Array();


$(document).ready(function () {




    /* initialize the external events
    -----------------------------------------------------------------*/
    InitializeEventList();



    /* initialize the calendar
    -----------------------------------------------------------------*/
    GetAllSavedEvents();

});

function InitializeEventList() {
    $('#external-events .fc-event').each(function () {

        // store data so the calendar knows to render an event upon drop
        $(this).data('event', {
            title: $.trim($(this).text()), // use the element's text as the event title
            stick: true // maintain when user navigates (see docs on the renderEvent method)
        });

        // make the event draggable using jQuery UI
        $(this).draggable({
            zIndex: 999,
            revert: true,      // will cause the event to go back to its
            revertDuration: 0  //  original position after the drag
        });

    });
}

function GetAllSavedEvents() {
    $.ajax({
        type: "POST",
        url: $('#hdn_GetAllSavedEvents').val(),//"@Url.Action("GetAllSavedEvents", "Dashboard")",
        data: {},
        success: function (data) {
            BindEvents(data);
        },
        error: function (arg) {
            alert("Error");
        }

    });
}

function BindEvents(data) {

    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        events: data,
        editable: true,
        droppable: true, // this allows things to be dropped onto the calendar
        slotEventOverlap: false,
        //eventOverlap: function (stillEvent, movingEvent) {
        //    return stillEvent.allDay && movingEvent.allDay;
        //},
        drop: function (date) {
            //Call when you drop any red/green/blue class to the week table.....first time runs only.....
            console.log("dropped");
            console.log(date.format());
            console.log(this.id);
            var defaultDuration = moment.duration($('#calendar').fullCalendar('option', 'defaultTimedEventDuration'));
            var end = date.clone().add(defaultDuration); // on drop we only have date given to us
            console.log('end is ' + end.format());
            console.log($.trim($(this).text()));
            console.log($.trim($(this).id));
            saveEvents(date.format(), end.format(), $.trim($(this).text()));
            RefreshAllEvents();
        },
        eventDrop: function (event, delta, revertFunc) {
            //inner column movement drop so get start and call the ajax function......
            console.log(event.start.format());
            console.log(event.id);
            var defaultDuration = moment.duration($('#calendar').fullCalendar('option', 'defaultTimedEventDuration'));
            var end = event.end || event.start.clone().add(defaultDuration);
            console.log('end is ' + end.format());
            UpdateEvent(event.start.format(), end.format(), event.id)

        },
        eventResize: function (event, delta, revertFunc) {
            console.log(event.id);
            console.log("Start time: " + event.start.format() + "end time: " + event.end.format());
            UpdateEvent(event.start.format(), event.end.format(), event.id)
        },
        eventRender: function (event, element) {
            SetLeftAndRightClick(event, event.id, element);
            
        },

        //eventClick: function(calEvent, jsEvent, view) {
        //    $(this).css('background-color', 'red');
        //$('.fc-content').removeClass('addeventbgcolor');
        //$(this).addClass('addeventbgcolor');
        //}
    });
}

function UpdateEvent(startDateTime, endDateTime, id) {
    $.ajax({
        type: "POST",
        url: $('#hdn_UpdateEvent').val(),// "@Url.Action("UpdateEvent", "Dashboard")",
        data: { startDateTime: startDateTime, endDateTime: endDateTime, id: id },
        success: function (data) {
            if (!data) {
                showfailure('Event updating failed');
            }
        },
        error: function (arg) {
            alert("Error");
        }

    });
}

function saveEvents(startDateTime, endDateTime, title) {
    console.log('saving called');
    $.ajax({
        type: "POST",
        url: $('#hdn_SaveEvent').val(),//"@Url.Action("SaveEvent", "Dashboard")",
        data: { startDateTime: startDateTime, endDateTime: endDateTime, title: title },
        success: function (data) {
            if (!data) {
                showfailure('Event saving failed');
            }
        },
        error: function (arg) {
            alert("Error");
        }

    });
}

//function RefreshAllEvents() {
//    var json = null;
//    $.ajax({
//        type: "POST",
//        url: $('#hdn_GetEvents').val(),// "@Url.Action("GetAllSavedEvents", "Dashboard")",
//        data: {},
//        success: function (data) {
//         
//            json = data;
//        },
//        error: function (arg) {
//        },
//        complete: function () {
//            $("#calendar").fullCalendar('removeEvents');
//            BindEvents(json);
//            $("#calendar").fullCalendar('addEventSource', json);
//        }
//    });
//}

function deleteEvent(id) {
    $.ajax({
        type: "POST",
        url: $('#hdn_Delete').val(),
        data: { ID: id },
        success: function (data) {
            if (!data) {
                showfailure('Delete Event  failed');
            }
            else {
               // RefreshAllEvents();
            }
        },
        error: function (arg) {
           
        }
    });
}

function Add(id) {
   
    $.ajax({
        type: "POST",
        url: $('#hdn_Add').val(),
        data: { ID: id },
        success: function (data) {
            if (!data) {
                showfailure('Add Event  failed');
            }
            else {
                // RefreshAllEvents();
            }
        },
        error: function (arg) {

        }
    });
}

function EditEvents(id) {
    
    $.ajax({
        type: "POST",
        url: $('#hdn_Add').val(),
        data: { ID: id },
        success: function (data) {
            if (!data) {
                showfailure('EditEvents  failed');
            }
            else {
                // RefreshAllEvents();
            }
        },
        error: function (arg) {

        }
    });
}

function Edit(id) {
   
    if (id > 0)
    {
        window.location.href = 'http://localhost:64259/Dashboard/AddOrEdit/' + id;
    }
   
}


function SetLeftAndRightClick(event, id, element) {
    element.bind('mousedown', function (e) {
        if (e.which == 3) {
            $(function () {
                $.contextMenu({
                    selector: '.fc-content',
                    items: {
                        "delete": { name: "Delete", icon: "delete" },
                        "edit": { name: "Edit", icon: "edit" },
                    },
                    callback: function (key, options) {
                        if (key.toLowerCase() == "delete")
                        {
                            deleteEvent(id);
                        }
                        else
                        {
                            Edit(id);
                        }
                    },
                });
            });

            $('Delete').on('click', function (e) {
                console.log('clicked', this);
            })
        }
    });

}


