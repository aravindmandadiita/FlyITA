/**
 * navigation.js — Responsive hamburger menu
 * Vanilla JS rewrite of legacy jQuery menumaker plugin (no jQuery dependency)
 */
document.addEventListener('DOMContentLoaded', () => {
    const nav = document.querySelector('.navigation');
    if (!nav) return;

    // Create hamburger button
    const menuBtn = document.createElement('div');
    menuBtn.id = 'menu-button';
    menuBtn.innerHTML = '<a class="mobile-menu-icon bar-icon" role="button" aria-expanded="false" aria-controls="globalmenu" aria-label="Toggle navigation"><span class="fa-solid fa-bars" aria-hidden="true"></span></a>';
    nav.insertBefore(menuBtn, nav.firstChild);

    const toggle = menuBtn.querySelector('a');
    const mainMenu = nav.querySelector('ul');

    // Toggle menu on hamburger click
    toggle.addEventListener('click', () => {
        const isOpen = menuBtn.classList.toggle('menu-opened');
        toggle.setAttribute('aria-expanded', isOpen);

        if (mainMenu) {
            if (mainMenu.classList.contains('open')) {
                mainMenu.style.display = 'none';
                mainMenu.classList.remove('open');
            } else {
                mainMenu.style.display = 'block';
                mainMenu.classList.add('open');
                // Show submenus in multitoggle mode
                mainMenu.querySelectorAll('ul').forEach(ul => {
                    ul.style.display = '';
                });
            }
        }
    });

    // Add submenu toggle buttons for dropdowns
    nav.querySelectorAll('li').forEach(li => {
        if (li.querySelector('ul')) {
            li.classList.add('nav-has-sub');
            const btn = document.createElement('span');
            btn.className = 'submenu-button';
            btn.setAttribute('role', 'button');
            btn.setAttribute('aria-expanded', 'false');
            btn.setAttribute('aria-haspopup', 'true');
            li.insertBefore(btn, li.firstChild);

            btn.addEventListener('click', () => {
                const subMenu = li.querySelector('ul');
                if (!subMenu) return;
                const isOpen = btn.classList.toggle('submenu-opened');
                btn.setAttribute('aria-expanded', isOpen);

                if (subMenu.classList.contains('open')) {
                    subMenu.classList.remove('open');
                    subMenu.style.display = 'none';
                } else {
                    subMenu.classList.add('open');
                    subMenu.style.display = 'block';
                }
            });
        }
    });

    // Add dropdown class
    nav.classList.add('dropdown');

    // Responsive resize handler
    function resizeFix() {
        if (window.innerWidth > 991) {
            nav.querySelectorAll('ul').forEach(ul => {
                ul.style.display = '';
            });
        } else if (window.innerWidth <= 991) {
            nav.querySelectorAll('ul').forEach(ul => {
                ul.style.display = 'none';
                ul.classList.remove('open');
            });
        }
    }

    resizeFix();
    window.addEventListener('resize', resizeFix);
});
