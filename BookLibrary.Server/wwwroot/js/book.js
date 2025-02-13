﻿function setupAutocomplete(inputId, apiUrl, dropdownId, selectedListId, displayField, hiddenInputName, initialItems) {
    $('#' + inputId).on('input', function () {
        const query = $(this).val();
        if (query.length >= 1) {
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

$(document).ready(function () {
    const addPagesButton = document.getElementById('addPage');
    // add <input type="text" name="Pages" class="form-control" /> to the form (before the button)
    addPagesButton.addEventListener('click', function () {
        const pageDiv = document.createElement('div');
        pageDiv.classList.add('page-content');
        const input = document.createElement('textarea');
        input.name = 'Pages';
        input.placeholder = 'Content of the page';
        input.rows = 10;
        input.classList.add('form-control');
        input.classList.add('mb-3');
        pageDiv.appendChild(input);

        // add delete button (float right)
        const deleteButton = document.createElement('button');
        deleteButton.type = 'button';
        deleteButton.classList.add('btn');
        deleteButton.classList.add('btn-danger');
        deleteButton.classList.add('page-delete-btn');
        deleteButton.textContent = 'Delete';
        deleteButton.addEventListener('click', function () {
            deletePage(deleteButton);
        });
        pageDiv.appendChild(deleteButton);

        addPagesButton.before(pageDiv);
    });
    
    // importFromIsbn
    const importFromISBN = isbn => {
        fetch("/Book/FetchFromISBN?isbn=" + isbn)
            .then(response => response.json())
            .then(data => {
                const nameInput = document.getElementById('Name');
                const descriptionInput = document.getElementById('Description');

                nameInput.value = data.name;
                descriptionInput.value = data.description;

                if (data.missingAuthors && data.missingAuthors.length > 0) {
                    console.log(data.missingAuthors);
                    if (confirm('Authors not found: ' + data.missingAuthors.map(x => x.firstName + ' ' + x.lastName).join(', ') + '. Create them?')) {
                        fetch('/Author/CreateMany', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({
                                authors: data.missingAuthors
                            })
                        }).then(data => {
                            // reimport
                            importFromISBN(isbn);
                        });
                        return;
                    }
                }
                if (data.foundAuthors) {
                    // remove all authors
                    const selectedAuthors = document.getElementById('selectedAuthors');
                    selectedAuthors.innerHTML = '';
                    // add authors
                    for (let i = 0; i < data.foundAuthors.length; i++) {
                        const author = data.foundAuthors[i];
                        selectedAuthors.innerHTML += '<div class="chip" data-id="' + author.id + '">' + author.firstName + ' ' + author.lastName + '<span class="closebtn" onclick="removeItem(this, \'AuthorIds\', \'' + author.id + '\');">&times;</span></div>';
                        selectedAuthors.innerHTML += '<input type="hidden" name="AuthorIds" value="' + author.id + '">';
                    }
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Error fetching book from ISBN');
            });
    }
    const importButton = document.getElementById('importFromIsbn');
    importButton.addEventListener('click', function () {
        // open popup to enter ISBN
        const isbn = prompt('Enter ISBN');
        if (isbn != null) {
            importFromISBN(isbn);
        } else {
            alert('No ISBN entered');
        }
    });
});

function deletePage(btn) {
    btn.parentElement.remove();
}