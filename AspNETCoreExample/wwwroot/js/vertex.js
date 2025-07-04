function executeStream(inp) {

    var id = uuid();
    var chunkSet = "";

    $('#messages').append(`<div class="d-flex"><div class="icon-circle"><i class="fa-regular fa-note-sticky"></i></div><span class="dot-flashing ms-4 mt-3"></span></div>`);
    $('#messages').append(`<div id="${id}"></div>`);
    $('#messages').scrollTop($('#messages')[0].scrollHeight);

    fetch('/api/vertex', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ input: inp })
    })
        .then(response => {
            const reader = response.body.getReader();
            const decoder = new TextDecoder();

            function typeCharacter(char) {
                return new Promise(resolve => {
                    setTimeout(() => {
                        $('#' + id).append(char);
                        $('#messages').scrollTop($('#messages')[0].scrollHeight);
                        resolve();
                    }, 10);
                });
            }

            async function readChunk() {
                const { done, value } = await reader.read();

                if (done) {

                    var htmlMarked = marked.parse(chunkSet);
                    $('#' + id).html(htmlMarked);
                    $('.dot-flashing').remove();
                    $('#messages').scrollTop($('#messages')[0].scrollHeight);
                    return;
                }

                const chunkText = decoder.decode(value, { stream: true });

                const lines = chunkText.split("\r\n");
                let filteredText = "";
                for (let i = 0; i < lines.length; i++) {
                    if (!/^[0-9a-fA-F]+$/.test(lines[i])) {
                        filteredText += lines[i];
                    }
                }

                chunkSet += filteredText;

                $('.dot-flashing').remove();

                $('#' + id).addClass('incoming-message');

                let typedText = '';
                for (let char of filteredText) {
                    typedText += char;
                    await typeCharacter(char);
                }

                const parsedHtml = marked.parse(typedText);
                $('#' + id).html($('#' + id).html().replace(typedText, parsedHtml));

                readChunk();
            }

            readChunk();
        })
        .catch(error => {
            $('#' + id).removeClass('dot-flashing')
                .removeClass('incoming-message')
                .addClass('text-danger');
            $('#' + id).html(error);
        });
}
