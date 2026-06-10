// Auto-dismiss alerts after 5 seconds
$(function () {
    window.setTimeout(function () {
        $(".alert-dismissible").fadeTo(500, 0).slideUp(500, function () {
            $(this).remove();
        });
    }, 5000);
});

// Clickable table rows: navigate to data-href, unless the click landed on a
// real link or button inside the row (so nested links keep their own targets).
$(function () {
    $(document).on('click', 'tr.clickable-row', function (e) {
        if ($(e.target).closest('a, button, input, label, .btn').length) {
            return;
        }
        var href = $(this).data('href');
        if (href) {
            window.location.href = href;
        }
    });

    // Keyboard accessibility: Enter activates a focused clickable row.
    $(document).on('keydown', 'tr.clickable-row', function (e) {
        if (e.key === 'Enter') {
            var href = $(this).data('href');
            if (href) {
                window.location.href = href;
            }
        }
    });
});
