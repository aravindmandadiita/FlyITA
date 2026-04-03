/**
 * site.js — Session timeout, phone formatting, forward nav, itinerary modal
 * Vanilla JS rewrite of legacy main.js (no jQuery dependency)
 */
document.addEventListener('DOMContentLoaded', () => {
    const body = document.body;

    // Forward navigation prevention
    if (body.dataset.disableForwardNav !== 'true') {
        try { history.forward(); } catch (e) { /* ignore */ }
    }

    // Scroll to window top if configured
    if (body.dataset.scrollToTop === 'true') {
        window.scrollTo(0, 0);
    }

    // Session timeout timer
    const timeoutTimer = new TimeoutTimer();
    timeoutTimer.init();

    // Phone formatting
    document.querySelectorAll('input[type=tel], input.telephone').forEach(input => {
        input.addEventListener('blur', formatPhone);
        formatPhone.call(input);
    });

    // Scroll-to-invalid-input override
    document.querySelectorAll('input, select, textarea').forEach(el => {
        el.addEventListener('invalid', () => {
            const invalid = document.querySelector('input:invalid, select:invalid, textarea:invalid');
            if (invalid) {
                invalid.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        });
    });

    // Itinerary modal — view itinerary button
    const viewBtn = document.getElementById('viewItinerary');
    if (viewBtn) {
        viewBtn.addEventListener('click', viewItinerary);
    }
});

/**
 * Opens Travelport ViewTrip in a new window
 */
function viewItinerary() {
    const reservationNumber = document.getElementById('ReservationNumber')?.value?.trim() || '';
    const travelerLastName = document.getElementById('TravelerLastName')?.value?.trim() || '';
    const url = `https://viewtrip.travelport.com/#!/itinerary?loc=${encodeURIComponent(reservationNumber)}&lName=${encodeURIComponent(travelerLastName)}`;
    window.open(url);
}

/**
 * Phone number formatting (XXX-XXX-XXXX)
 */
function formatPhone() {
    const input = this instanceof HTMLInputElement ? this : this;
    let value = input.value.trim();
    input.value = value;

    const formGroup = input.closest('.form-group');
    if (formGroup) formGroup.classList.remove('has-error');

    if (value.length === 0) return;

    const expectedLength = 12;
    const regex = /\d{3}-\d{3}-\d{4}/g;

    if (value.length === expectedLength && regex.test(value)) return;

    const digits = value.replace(/\D/g, '');
    let formatted = digits.substr(0, 3);
    if (digits.length > 3) formatted += '-' + digits.substr(3, 3);
    if (digits.length > 6) formatted += '-' + digits.substr(6, 4);
    input.value = formatted;

    if (formatted.length !== expectedLength || !/\d{3}-\d{3}-\d{4}/.test(formatted)) {
        if (formGroup) formGroup.classList.add('has-error');
    }
}

/**
 * Session timeout timer with warning modal
 */
class TimeoutTimer {
    constructor() {
        this.timeoutSeconds = Number(document.body.dataset.timeoutSeconds) || 0;
        this.warningSeconds = Number(document.body.dataset.timeoutWarningSeconds) || 0;
        this.remainingSeconds = 0;
        this.timer = null;
        this.timeout = null;
        this.modal = null;
    }

    init() {
        if (!this.timeoutSeconds || !this.warningSeconds) return;

        this.remainingSeconds = this.timeoutSeconds;
        const timerEl = document.getElementById('timeout_timer');
        if (timerEl) timerEl.textContent = this.remainingSeconds;

        this.timeout = setTimeout(() => this.showWarning(), this.warningSeconds * 1000);

        const extendBtn = document.getElementById('extend-session');
        if (extendBtn) {
            extendBtn.addEventListener('click', () => this.refreshSession());
        }
    }

    showWarning() {
        if (document.getElementById('PreventTimeoutModal')) return;

        const modalEl = document.getElementById('timeout_warning');
        if (modalEl && typeof bootstrap !== 'undefined') {
            this.modal = new bootstrap.Modal(modalEl, { keyboard: false, backdrop: 'static' });
            this.modal.show();
            this.timer = setInterval(() => this.updateTimer(), 1000);
        }
    }

    updateTimer() {
        this.remainingSeconds--;
        const timerEl = document.getElementById('timeout_timer');
        if (timerEl) timerEl.textContent = this.remainingSeconds;

        if (this.remainingSeconds < 1) {
            window.location.href = 'logout.aspx?msg=timeout';
        }
    }

    refreshSession() {
        fetch('/api/session/extend').catch(() => { /* no-op until endpoint exists */ });
        this.clearTimer();
        if (this.modal) this.modal.hide();
    }

    clearTimer() {
        this.remainingSeconds = this.timeoutSeconds;
        const timerEl = document.getElementById('timeout_timer');
        if (timerEl) timerEl.textContent = this.remainingSeconds;
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
    }
}
