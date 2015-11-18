
$(function () {
    populateDiv(0);
    $('input[type=file]').change(function () {
        var objs = $('input[type=file]');
        for (var i = 0; i < objs.length; i++)
        {
            if (objs.eq(i).val() == "") {
                objs.eq(i).width("190px");
            }
            else {
                objs.eq(i).width("100%");
            }
        }
    })
});

function populateDiv(actualLength)
{
    $.get('getFiles', function (data) {
        var files = $(data).find('string');
        var length = files.length;
        if (actualLength != length)
        {
            $('#links').empty();
            for (var i = 0; i < length; i++) {
                var fileName = files.eq(i).text();
                $('#links').append('<a href="files/' + fileName + '">' + fileName + '</a><br/>');
            }
        }
        setTimeout(function () {
            populateDiv(length)
        }, 10000);
    })

}