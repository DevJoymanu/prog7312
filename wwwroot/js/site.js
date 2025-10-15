// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Site-wide JavaScript functionality

document.addEventListener('DOMContentLoaded', function () {
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            alert.style.transform = 'translateY(-20px)';
            setTimeout(() => {
                if (alert.parentNode) {
                    alert.parentNode.removeChild(alert);
                }
            }, 300);
        }, 5000);
    });

    // Form validation enhancements
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const submitButton = form.querySelector('button[type="submit"]');
            if (submitButton) {
                submitButton.disabled = true;
                submitButton.textContent = submitButton.textContent.replace('Submit', 'Submitting...');

                // Re-enable button after 3 seconds to prevent permanent disable
                setTimeout(() => {
                    submitButton.disabled = false;
                    submitButton.textContent = submitButton.textContent.replace('Submitting...', 'Submit');
                }, 3000);
            }
        });
    });

    // Photo preview functionality
    const photoInputs = document.querySelectorAll('input[type="file"][accept*="image"]');
    photoInputs.forEach(input => {
        input.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    // Remove existing preview
                    const existingPreview = input.parentNode.querySelector('.photo-preview');
                    if (existingPreview) {
                        existingPreview.remove();
                    }

                    // Create new preview
                    const preview = document.createElement('div');
                    preview.className = 'photo-preview';
                    preview.innerHTML = `
                        <img src="${e.target.result}" alt="Photo preview" style="max-width: 200px; max-height: 200px; border-radius: 8px; margin-top: 0.5rem; border: 1px solid var(--border-color);">
                        <button type="button" class="remove-preview" style="display: block; margin-top: 0.5rem; padding: 0.25rem 0.5rem; background: rgba(239,68,68,0.2); color: #ef4444; border: 1px solid rgba(239,68,68,0.3); border-radius: 6px; cursor: pointer;">Remove</button>
                    `;

                    input.parentNode.appendChild(preview);

                    // Add remove functionality
                    preview.querySelector('.remove-preview').addEventListener('click', function () {
                        input.value = '';
                        preview.remove();
                    });
                };
                reader.readAsDataURL(file);
            }
        });
    });

    // Add loading animation to service cards
    const serviceCards = document.querySelectorAll('.service-card.enabled');
    serviceCards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.1}s`;
        card.addEventListener('click', function () {
            if (!card.classList.contains('disabled')) {
                card.style.transform = 'scale(0.95)';
                setTimeout(() => {
                    card.style.transform = '';
                }, 150);
            }
        });
    });

    // Smooth scroll for anchor links
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    anchorLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add ripple effect to buttons
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            const rect = button.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.cssText = `
                position: absolute;
                border-radius: 50%;
                background: rgba(255,255,255,0.3);
                transform: scale(0);
                animation: ripple 0.6s linear;
                left: ${x}px;
                top: ${y}px;
                width: ${size}px;
                height: ${size}px;
                pointer-events: none;
            `;

            if (!button.style.position || button.style.position === 'static') {
                button.style.position = 'relative';
            }

            button.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });

    // Add CSS for ripple animation
    if (!document.getElementById('ripple-styles')) {
        const style = document.createElement('style');
        style.id = 'ripple-styles';
        style.textContent = `
            @keyframes ripple {
                to {
                    transform: scale(4);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
});