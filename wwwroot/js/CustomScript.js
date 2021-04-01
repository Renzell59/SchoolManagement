var confirmDelete = function (buttonId, isConfirmDelete) {
    let deleteId = `deleteId_${buttonId}`;
    let confirmDelete = `confirmDelete_${buttonId}`

    if (isConfirmDelete) {
        $(`#${deleteId}`).hide();
        $(`#${confirmDelete}`).show();
    }
    else {
        $(`#${confirmDelete}`).hide();
        $(`#${deleteId}`).show();
    };
};