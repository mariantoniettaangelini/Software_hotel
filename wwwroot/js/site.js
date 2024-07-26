// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let basePath = '/api/Prenotazioni';
$(() => {
    GetPrenotazioniByCF();
    GetPrenotazioniByPensione();

    $("#search").on('click', () => {
        let codiceFiscale = $("#codiceFiscale").val();
        $.ajax({
            url: `${basePath}/CodiceFiscale/${codiceFiscale}`,
            method: 'get',
            success: function (data) {
                let results = $("#results");
                results.empty();
                data.forEach(p => {
                    results.append(`<p>Prenotazione ID: ${p.idPrenotazione} - Camera: ${p.camera.descrizioneStanza}</p>`);
                });
            },
            error: (e) => console.log(e)
        });
    });

    $("#searchPensione").on('click', () => {
        $.ajax({
            url: `${basePath}/PensioneCompleta`,
            method: 'get',
            success: function (numeroPrenotazioni) {
                $("#resultsPensione").text(`Numero di prenotazioni per pensione completa: ${numeroPrenotazioni}`);
            },
            error: (e2) => console.log(e2)

        });
    });
})
