document.addEventListener("DOMContentLoaded", function () {
    const profile = document.querySelector('.profile');
    const dropdown = document.querySelector('.dropdown__wrapper');

    profile.addEventListener('click', () => {
        dropdown.classList.remove('none');
        dropdown.classList.toggle('hide');
    })

    document.addEventListener("click", (event) => {
        const isClickInsideDropdown = dropdown.contains(event.target);
        const isProfileClicked = profile.contains(event.target);

        if (!isClickInsideDropdown && !isProfileClicked) {
            dropdown.classList.add('hide');
            dropdown.classList.remove('dropdown__wrapper--fade-in');
        }
    });
});