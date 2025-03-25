$(document).ready(function () {
    // Tìm kiếm theo UserName
    $('#searchBox').on('input', function () {
        var searchText = $(this).val().toLowerCase();
        $('.account-row').each(function () {
            var username = $(this).data('username');
            $(this).toggle(username.includes(searchText));
        });
    });

    // Lọc trạng thái tài khoản
    $('#statusFilter').on('change', function () {
        var selectedStatus = $(this).val();
        $('.account-row').each(function () {
            var accountStatus = $(this).data('status');
            $(this).toggle(selectedStatus === 'all' || accountStatus === selectedStatus);
        });
    });

    // Xóa tài khoản với popup xác nhận
    $('.delete-account').click(function () {
        var accountId = $(this).data('id');

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Admin/Accounts/Delete/' + accountId,
                    type: 'POST',
                    data: {
                        id: accountId,
                        __RequestVerificationToken: $('#deleteAccountForm input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                title: "Deleted!",
                                text: "Account has been deleted.",
                                icon: "success",
                                timer: 2000,
                                showConfirmButton: false
                            }).then(() => {
                                location.reload();
                            });
                        } else {
                            Swal.fire({
                                title: "Error!",
                                text: response.message,
                                icon: "error"
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            title: "Error!",
                            text: "Error deleting account. Please try again.",
                            icon: "error"
                        });
                    }
                });
            }
        });
    });

    // Cập nhật trạng thái Active
    $('.toggle-active').change(function () {
        var accountId = $(this).data('id');
        var isActive = $(this).prop('checked');

        $.ajax({
            url: '/Admin/Accounts/ActiveStatus', // Gọi API mới
            type: 'POST',
            data: {
                id: accountId,
                isActive: isActive,
                __RequestVerificationToken: $('#deleteAccountForm input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        title: "Success!",
                        text: "User status updated successfully.",
                        icon: "success",
                        timer: 1500,
                        showConfirmButton: false
                    });

                    // Cập nhật trạng thái data-status của hàng (để lọc hoạt động đúng)
                    var row = $('.account-row[data-id="' + accountId + '"]');
                    row.attr('data-status', isActive ? 'active' : 'inactive');
                } else {
                    Swal.fire({
                        title: "Error!",
                        text: response.message,
                        icon: "error"
                    });
                }
            },
            error: function () {
                Swal.fire({
                    title: "Error!",
                    text: "Failed to update status. Please try again.",
                    icon: "error"
                });
            }
        });
    });
});
