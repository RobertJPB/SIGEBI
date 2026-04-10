/**
 * SIGEBI Time Localization Helper
 * Convierte fechas en formato ISO (UTC) a la hora local del navegador del usuario.
 */
document.addEventListener('DOMContentLoaded', function() {
    localizeTimes();
});

function localizeTimes() {
    const timeElements = document.querySelectorAll('.local-time');
    
    timeElements.forEach(el => {
        const utcDateStr = el.getAttribute('data-utc');
        if (!utcDateStr) return;

        try {
            const date = new Date(utcDateStr);
            
            // Si el elemento tiene un atributo de formato específico
            const format = el.getAttribute('data-format');
            
            let formattedDate = "";
            
            if (format === 'short') {
                formattedDate = new Intl.DateTimeFormat(undefined, {
                    day: 'numeric',
                    month: 'short',
                    year: 'numeric'
                }).format(date);
            } else if (format === 'full') {
                 formattedDate = new Intl.DateTimeFormat(undefined, {
                    day: 'numeric',
                    month: 'short',
                    year: 'numeric',
                    hour: 'numeric',
                    minute: 'numeric',
                    hour12: true
                }).format(date);
            } else {
                // Formato por defecto: día corto, mes corto, año
                formattedDate = new Intl.DateTimeFormat(undefined, {
                    day: 'numeric',
                    month: 'short',
                    year: 'numeric'
                }).format(date);
            }

            el.textContent = formattedDate;
            el.classList.remove('invisible'); // Si estaba oculto para evitar salto visual
        } catch (e) {
            console.error("Error localizing date:", utcDateStr, e);
        }
    });
}
