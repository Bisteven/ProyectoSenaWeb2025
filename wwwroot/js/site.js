// Efectos de animación al cargar la página
document.addEventListener('DOMContentLoaded', function() {
    // Animación de entrada para las filas de la tabla
    const tableRows = document.querySelectorAll('.table tbody tr');
    tableRows.forEach((row, index) => {
        row.style.opacity = '0';
        row.style.transform = 'translateY(20px)';
        setTimeout(() => {
            row.style.transition = 'all 0.5s ease';
            row.style.opacity = '1';
            row.style.transform = 'translateY(0)';
        }, index * 50);
    });

    // Efecto de hover mejorado en las cards
    const statsCards = document.querySelectorAll('.stats-card');
    statsCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-8px) scale(1.02)';
        });
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Efecto de ripple en botones
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple');
            
            this.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });

    // Animación de contador para estadísticas
    const statNumbers = document.querySelectorAll('.stats-card h2');
    statNumbers.forEach(stat => {
        const target = parseInt(stat.textContent);
        if (!isNaN(target)) {
            let current = 0;
            const increment = target / 30;
            const timer = setInterval(() => {
                current += increment;
                if (current >= target) {
                    stat.textContent = target;
                    clearInterval(timer);
                } else {
                    stat.textContent = Math.floor(current);
                }
            }, 30);
        }
    });

    // Efecto parallax sutil en el fondo
    window.addEventListener('scroll', function() {
        const scrolled = window.pageYOffset;
        const parallax = document.querySelector('body::before');
        if (parallax) {
            document.body.style.backgroundPosition = `center ${scrolled * 0.5}px`;
        }
    });

    // Validación de formularios con feedback visual
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const inputs = form.querySelectorAll('input[required], select[required]');
            let isValid = true;
            
            inputs.forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;
                    input.classList.add('is-invalid');
                    setTimeout(() => {
                        input.classList.remove('is-invalid');
                    }, 3000);
                } else {
                    input.classList.add('is-valid');
                }
            });
        });
    });

    // Efecto de typing en títulos (opcional, para efectos visuales)
    const titles = document.querySelectorAll('h2');
    titles.forEach(title => {
        const text = title.textContent;
        title.textContent = '';
        title.style.opacity = '0';
        
        setTimeout(() => {
            let index = 0;
            const typeInterval = setInterval(() => {
                if (index < text.length) {
                    title.textContent += text.charAt(index);
                    index++;
                } else {
                    clearInterval(typeInterval);
                    title.style.opacity = '1';
                }
            }, 50);
        }, 200);
    });
});

// Agregar estilos para el efecto ripple
const style = document.createElement('style');
style.textContent = `
    .btn {
        position: relative;
        overflow: hidden;
    }
    
    .ripple {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.6);
        transform: scale(0);
        animation: ripple-animation 0.6s ease-out;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
    
    .is-invalid {
        border-color: #dc3545 !important;
        animation: shake 0.5s;
    }
    
    .is-valid {
        border-color: #28a745 !important;
    }
    
    @keyframes shake {
        0%, 100% { transform: translateX(0); }
        25% { transform: translateX(-10px); }
        75% { transform: translateX(10px); }
    }
`;
document.head.appendChild(style);
