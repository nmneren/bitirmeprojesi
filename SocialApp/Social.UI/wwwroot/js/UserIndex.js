$('#myTab a').on('click', function (event) {
    event.preventDefault()
    $(this).tab('show')
})