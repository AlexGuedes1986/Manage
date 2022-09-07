$(function () {
    $('select[name="Country"]').change(function () {
        let countryCode = $(this).val();
        let url = $(this).data('url');  
        $.ajax({
            type: 'post',
            url: url,
            data: { doNotGetCountries: true, codeCountry: countryCode },
            success: function () {
                console.log('on success');
            },
            error: function () {
                console.log('on error');
            }
        })
    })

    $('select[name="League"]').change(function () {
        let league = $(this).val();
        let url = $(this).data('url');   
        $.ajax({
            type: 'post',
            url: url,
            data: { doNotGetCountries: true, league : league },
            success: function () {
                console.log('on success');
            },
            error: function () {
                console.log('on error');
            }
        })
    })

    $('select[name="Team"]').change(function () {
        let teamId = $(this).val();
        let url = $(this).data('url');
        $.ajax({
            type: 'post',
            url: url,
            data: { doNotGetCountries: true, league: league },
            success: function () {
                console.log('on success');
            },
            error: function () {
                console.log('on error');
            }
        })
    })


})