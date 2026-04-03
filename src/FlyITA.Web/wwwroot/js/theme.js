/**
 * theme.js — Sticky header, scroll-to-top, parallax
 * Vanilla JS rewrite of legacy jQuery theme scripts (no jQuery dependency)
 */
document.addEventListener('DOMContentLoaded', () => {
    const header = document.querySelector('.header');
    const scrollTopBtn = document.querySelector('.scroll-top');

    // Sticky header on scroll
    window.addEventListener('scroll', () => {
        if (header) {
            header.classList.toggle('sticky', window.scrollY > 1);
        }

        // Show/hide scroll-to-top button
        if (scrollTopBtn) {
            if (window.scrollY > 300) {
                scrollTopBtn.style.display = 'block';
            } else {
                scrollTopBtn.style.display = 'none';
            }
        }
    });

    // Scroll-to-top click handler
    if (scrollTopBtn) {
        scrollTopBtn.addEventListener('click', (e) => {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // Scroll-down links
    document.querySelectorAll('.scroll-down[href^="#"], .scroll-to-target[href^="#"]').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const target = document.querySelector(link.getAttribute('href'));
            if (target) {
                target.scrollIntoView({ behavior: 'smooth' });
            }
        });
    });

    // Parallax scrolling
    const parallaxElements = document.querySelectorAll('.parallax');
    if (parallaxElements.length > 0) {
        let ticking = false;

        function updateParallax() {
            const scrollTop = window.scrollY;

            parallaxElements.forEach(el => {
                const yPos = scrollTop * 0.5;

                if (el.classList.contains('parallax-section1')) {
                    el.style.top = yPos + 'px';
                }
                if (el.classList.contains('parallax-section2')) {
                    el.style.top = -yPos + 'px';
                }
                if (el.classList.contains('parallax-static')) {
                    el.style.top = scrollTop + 'px';
                }
                if (el.classList.contains('parallax-opacity')) {
                    const parallaxHeight = el.offsetHeight;
                    el.style.opacity = Math.max(0, Math.min(1, 1 - (scrollTop / parallaxHeight)));
                }
                if (el.classList.contains('parallax-background1')) {
                    el.style.backgroundPosition = 'left ' + yPos + 'px';
                }
                if (el.classList.contains('parallax-background2')) {
                    el.style.backgroundPosition = 'left ' + -yPos + 'px';
                }
            });

            ticking = false;
        }

        window.addEventListener('scroll', () => {
            if (!ticking) {
                requestAnimationFrame(updateParallax);
                ticking = true;
            }
        });
    }

    // Itinerary modal focus management (Bootstrap 5 events)
    const modalItinerary = document.getElementById('modalItinerary');
    if (modalItinerary) {
        modalItinerary.addEventListener('shown.bs.modal', () => {
            const firstInput = modalItinerary.querySelector('input');
            if (firstInput) firstInput.focus();
        });
    }
});
