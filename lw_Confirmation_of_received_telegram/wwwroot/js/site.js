// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// 一覧のスクロール枠（.sticky-header-scroll ＝食事一覧・マスタ保守）の高さ合わせ。
// CSS側の calc(100vh - 250px) は固定値のため、ツールバーの実際の高さと合わず画面下に空白が残る。
// 枠の上端の実位置から「画面下端まで」を計算して max-height を上書きする。
// height ではなく max-height にするのは、行が少ないとき枠が縮む動き（CSSと同じ）を保つため。
(function () {
    function fitStickyScrollHeight() {
        document.querySelectorAll('.sticky-header-scroll').forEach(function (el) {
            // ページ先頭からの枠の上端位置（スクロール中のresizeでもズレないよう文書基準で測る）
            var topInDocument = el.getBoundingClientRect().top + window.scrollY;
            var bottomGap = 16; // main の下余白（pb-3 = 1rem）のぶんだけ残す
            var height = window.innerHeight - topInDocument - bottomGap;
            // 小さすぎる値はCSSの min-height(10rem) が優先されるので、ここでは下限を設けない
            el.style.maxHeight = height + 'px';
        });
    }
    document.addEventListener('DOMContentLoaded', fitStickyScrollHeight);
    window.addEventListener('resize', fitStickyScrollHeight);
})();
