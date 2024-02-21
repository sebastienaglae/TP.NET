function setupAutocomplete(inputId, apiUrl, dropdownId, selectedListId, displayField, hiddenInputName, initialItems) {
    $('#' + inputId).on('input', function () {
        const query = $(this).val();
        if (query.length > 1) {
            $('#' + dropdownId).show();
            $.ajax({
                url: apiUrl,
                data: {query: query},
                success: function (response) {
                    const dropdown = $('#' + dropdownId);
                    dropdown.empty();
                    if (response.data.length > 0) {
                        for (let i = 0; i < response.data.length; i++) {
                            const item = response.data[i];
                            dropdown.append('<a class="dropdown-item" href="#" data-id="' + item.id + '">' + displayField(item) + '</a>');
                        }
                    } else {
                        dropdown.hide();
                    }
                }
            });
        } else {
            $('#' + dropdownId).hide();
        }
    });
    $('#' + dropdownId).on('click', '.dropdown-item', function (e) {
        e.preventDefault();
        const itemName = $(this).text();
        const itemId = $(this).data('id');
        if ($('#' + selectedListId + ' .chip[data-id="' + itemId + '"]').length == 0) {
            $('#' + selectedListId).append('<div class="chip" data-id="' + itemId + '">' + itemName + '<span class="closebtn" onclick="removeItem(this, \'' + hiddenInputName + '\', \'' + itemId + '\');">&times;</span></div>');
            $('#' + selectedListId).append('<input type="hidden" name="' + hiddenInputName + '" value="' + itemId + '">');
        }
        $('#' + inputId).val('');
        $('#' + dropdownId).hide();
    });
    if (initialItems != null)
        for (let i = 0; i < initialItems.length; i++) {
            const item = initialItems[i];
            console.log(item);
            $('#' + selectedListId).append('<div class="chip" data-id="' + item.key + '">' + item.value + '<span class="closebtn" onclick="removeItem(this, \'' + hiddenInputName + '\', \'' + item.key + '\');">&times;</span></div>');
            $('#' + selectedListId).append('<input type="hidden" name="' + hiddenInputName + '" value="' + item.key + '">');
        }
}

function removeItem(element, hiddenInputName, itemId) {
    $(element).parent().remove();
    $('input[name="' + hiddenInputName + '"][value="' + itemId + '"]').remove();
}

$(document).ready(function () {
    setupAutocomplete('Authors', '/api/authors', 'authorList', 'selectedAuthors', function (author) {
        return author.firstName + ' ' + author.lastName;
    }, 'AuthorIds', authors);
    setupAutocomplete('Genres', '/api/genres', 'genreList', 'selectedGenres', function (genre) {
        return genre.name;
    }, 'GenreIds', genres);
});