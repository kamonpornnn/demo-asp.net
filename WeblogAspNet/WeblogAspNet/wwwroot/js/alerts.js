document.addEventListener('DOMContentLoaded', function () {
  
    function getModalWidth() {
        if (window.innerWidth <= 480) {
            return '90%';
        } else if (window.innerWidth <= 768) {
            return '70%';
        } else {
            return '30%';
        }
    }

    if (successMessage) {
        Swal.fire({
            title: 'สำเร็จ!',
            text: successMessage,
            icon: 'success',
            timer: 5000,
            showConfirmButton: false,
            width: getModalWidth(),
            heightAuto: false,
        });
    }

    if (systemErrorMessage) {
        Swal.fire({
            title: 'ระบบมีปัญหา!',
            text: systemErrorMessage,
            icon: 'warning',
            timer: 5000,
            showConfirmButton: false,
            width: getModalWidth(),
            heightAuto: false,
        });
    }

    if (errorMessage) {
        Swal.fire({
            title: 'ข้อผิดพลาด!',
            text: errorMessage,
            icon: 'error',
            timer: 5000,
            showConfirmButton: false,
            width: getModalWidth(),
            heightAuto: false,
        });
    }
});
