
var dt;

$(document).ready(
    function () { LoadTable(); });

function LoadTable() {
    dt = $("#MyTable").DataTable({
        "ajax": {
            "url" : "/Admin/Product/GetData"
        },
        "columns": [
            { "data": "name" },
            { "data": "description" },
            { "data": "price" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <a href="/Admin/Product/Update/${data}" class="btn btn-info">Update</a>
                    <a onClick=DeleteItem("/Admin/Product/Delete/${data}") class="btn btn-danger">Delete</a>
                    `
                }
            }
        ]
    });
}


function DeleteItem(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        dt.ajax.reload();
                        toastr["success"](data.message);
                    } else {
                        toastr["error"](data.message);
                    }
                }
            });


        }
    });
}

