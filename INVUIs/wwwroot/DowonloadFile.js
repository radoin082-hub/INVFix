
    function openPdf(byteArray) {
    const blob = new Blob([new Uint8Array(byteArray)], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank');
}
