$(document).ready(function () {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/songhub")
        //.configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveSongUpdate", function (action, songId) {
        console.log("Received song update:", action, songId);
        if (action === "add") {
            window.location.href = "/Song/Index";
        } else if (action === "edit") {
            window.location.href = "/Song/Index";
        }else if (action === "delete") {
            window.location.href = "/Song/Index";
        } 
    });
    connection.start().then(function () {
        console.log("SignalR connected.");
    }).catch(function (err) {
        console.error(err.toString());
    });


    $("#addRowsBtn").click(function () {
        addNewRows();
    });


    // Check if any checkbox is checked
    $('body').on('change', '.extractDetailsCheckbox', function () {
        var anyChecked = $('.extractDetailsCheckbox:checked').length > 0;
        $('#extractPdfBtn').prop('disabled', !anyChecked);
    });

    $('#selectAllCheckbox').change(function () {
        var isChecked = $(this).prop('checked');

        // Get the DataTable instance
        var table = $('.datatable').DataTable();

        // Get the current page index
        var currentPageIndex = table.page();

        // Toggle checkboxes on the current page
        table.page(currentPageIndex).$('tr').each(function () {
            $(this).find('.extractDetailsCheckbox').prop('checked', isChecked);
        });
        // Update Extract Pdf button status based on checkboxes on the current page
        updateExtractPdfButtonStatus();
    });

    // Click event handler for the Extract Pdf button
    $('#extractPdfBtn').click(function () {
        var selectedSongs = [];
        $('.extractDetailsCheckbox:checked').each(function () {
            var songId = $(this).data('songid');
            selectedSongs.push(songId);
        });
        if (selectedSongs.length > 0) {
            // Redirect to ExtractPdf action with selected song ids
            window.location.href = '/SongView/ExtractPdf?selectedSongs=' + selectedSongs.join(',');
        }
    });

    $(".datatable").DataTable({
        "paging": true,
        "lengthMenu": [[8, 16, 24, -1], [8, 16, 24, "All"]],
        "pageLength": 8,
    });
    
    // Set the popover content dynamically
    $('.delete-song').popover({
        html: true,
        sanitize: false,
        content: function () {
            var songId = $(this).data('songid');
            return '<div class="popover-body">' +
                '<button type="button" class="btn btn-success btn-delete-yes" data-songid="' + songId + '">Yes</button> ' +
                '<button type="button" class="btn btn-danger btn-delete-no">No</button></div>';
        },
        //placement: 'top' // Show the popover above the button

    });

    $(document).on('click', '.btn-delete-yes', function () {
        var songId = $(this).data('songid');
        handleDeletion(songId);
    });


    $(document).on('click', '.btn-delete-no', function () {
        $(this).closest('.popover').popover('hide');
    });

    // Close popover when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.popover').length && !$(e.target).closest('.delete-song').length) {
            $('.delete-song').popover('hide');
        }
    });

    

    

});

function addNewRows() {
    $.ajax({
        url: '/SongView/AddNewSongViewRows',
        type: 'POST',
        success: function () {
            // Reload the page after adding new rows
            window.location.reload();
        },
        error: function () {
            alert('Error adding new rows');
        }
    });
}

function updateExtractPdfButtonStatus() {
    var anyChecked = $('.extractDetailsCheckbox:checked').length > 0;
    $('#extractPdfBtn').prop('disabled', !anyChecked);
}

// Function to handle deletion
function handleDeletion(songId) {
    $.ajax({
        url: '/Song/DeletePopover',
        type: 'POST',
        data: { id: songId },
        success: function (response) {
            if (response.success) {
                connection.invoke("SendSongUpdate", "delete", songId);
                //window.location.reload();
            } else {
                alert('Error deleting the song.');
            }
        },
        error: function () {
            alert('Error deleting the song.');
        }
    });
}


