
// Theme toggle functionality
document.getElementById('theme-toggle').addEventListener('change', function () {
    document.body.classList.toggle('dark-mode');
    localStorage.setItem('darkMode', document.body.classList.contains('dark-mode'));
});

// Check for saved theme preference
if (localStorage.getItem('darkMode') === 'true') {
    document.body.classList.add('dark-mode');
    document.getElementById('theme-toggle').checked = true;
}

// Table sorting function
function sortTable(n) {
    const table = document.getElementById("requests-table");
    let rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    switching = true;
    dir = "asc";

    while (switching) {
        switching = false;
        rows = table.rows;

        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("td")[n];
            y = rows[i + 1].getElementsByTagName("td")[n];

            if (dir === "asc") {
                if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                    shouldSwitch = true;
                    break;
                }
            } else if (dir === "desc") {
                if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                    shouldSwitch = true;
                    break;
                }
            }
        }

        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            switchcount++;
        } else {
            if (switchcount === 0 && dir === "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
}

// Export to PDF function
function exportToPDF(elementId = 'requests-table') {
    // Load html2pdf if not already loaded
    if (typeof html2pdf === 'undefined') {
        loadScript('https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js', () => {
            performPDFExport(elementId);
        });
    } else {
        performPDFExport(elementId);
    }
}

function performPDFExport(elementId) {
    const element = document.getElementById(elementId);
    const opt = {
        margin: 10,
        filename: `admin_report_${new Date().toISOString().slice(0, 10)}.pdf`,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: {
            scale: 2,
            logging: true,
            useCORS: true
        },
        jsPDF: {
            unit: 'mm',
            format: 'a4',
            orientation: 'portrait'
        }
    };

    html2pdf().set(opt).from(element).save();
}

// Export to Excel function
function exportToExcel() {
    // Load SheetJS if not already loaded
    if (typeof XLSX === 'undefined') {
        loadScript('https://cdn.jsdelivr.net/npm/xlsx@0.18.5/dist/xlsx.full.min.js', () => {
            performExcelExport();
        });
    } else {
        performExcelExport();
    }
}

function performExcelExport() {
    const table = document.getElementById('requests-table');
    const workbook = XLSX.utils.table_to_book(table);
    XLSX.writeFile(workbook, `admin_report_${new Date().toISOString().slice(0, 10)}.xlsx`);
}

// Helper function to load scripts dynamically
function loadScript(url, callback) {
    const script = document.createElement('script');
    script.src = url;
    script.onload = callback;
    document.head.appendChild(script);
}

function openNav() {
    document.getElementById("mySidepanel").style.width = "250px";
}

function closeNav() {
    document.getElementById("mySidepanel").style.width = "0";
}

// Optional: close sidebar when clicking outside
document.addEventListener('click', function (event) {
    if (!document.getElementById('mySidepanel').contains(event.target) &&
        !event.target.classList.contains('mobile-toggle')) {
        closeNav();
    }
});


