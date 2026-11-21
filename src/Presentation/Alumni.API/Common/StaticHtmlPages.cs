namespace Alumni.API.Common;

public static class StaticHtmlPages
{
    private static string RenderPageShell(string pageTitle, string activePath, string extraCss, string mainContent)
    {
        string NavClass(string path) => activePath == path ? "nav-link active" : "nav-link";

        return $$"""
        <!DOCTYPE html>
        <html lang="tr">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>{{pageTitle}}</title>
            <link rel="preconnect" href="https://fonts.googleapis.com">
            <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
            <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&family=JetBrains+Mono:wght@500&family=Plus+Jakarta+Sans:wght@600;700;800&display=swap" rel="stylesheet">
            <style>
                :root {
                    --bg-canvas: #F8FAFC;
                    --bg-surface: #FFFFFF;
                    --bg-surface-subtle: #F1F5F9;
                    --brand-navy: #1E3A8A;
                    --brand-navy-hover: #1E40AF;
                    --brand-navy-deep: #172554;
                    --brand-navy-tint: #EFF6FF;
                    --accent-gold: #D97706;
                    --accent-gold-hover: #B45309;
                    --accent-gold-tint: #FEF3C7;
                    --text-primary: #1E293B;
                    --text-secondary: #475569;
                    --text-muted: #64748B;
                    --border-subtle: #E2E8F0;
                    --border-strong: #CBD5E1;
                    --status-success: #059669;
                    --status-success-tint: #ECFDF5;
                    --status-warning: #D97706;
                    --status-warning-tint: #FFFBEB;
                    --status-danger: #DC2626;
                    --status-danger-tint: #FEF2F2;
                    --shadow-sm: 0 1px 2px rgba(30, 41, 59, 0.04), 0 1px 3px rgba(30, 41, 59, 0.03);
                    --shadow-md: 0 10px 25px -5px rgba(30, 58, 138, 0.08), 0 4px 10px -2px rgba(30, 41, 59, 0.04);
                    --shadow-nav: 0 4px 16px rgba(15, 23, 42, 0.06);
                    --radius-sm: 6px;
                    --radius-md: 8px;
                    --radius-lg: 12px;
                    --radius-pill: 9999px;
                }

                *, *::before, *::after {
                    box-sizing: border-box;
                }

                html {
                    scroll-behavior: smooth;
                }

                body {
                    margin: 0;
                    font-family: 'Inter', system-ui, -apple-system, sans-serif;
                    font-size: 16px;
                    line-height: 1.6;
                    background-color: var(--bg-canvas);
                    color: var(--text-primary);
                    -webkit-font-smoothing: antialiased;
                    min-height: 100vh;
                    display: flex;
                    flex-direction: column;
                }

                main {
                    flex: 1;
                }

                h1, h2, h3, .font-display {
                    font-family: 'Plus Jakarta Sans', 'Inter', system-ui, -apple-system, sans-serif;
                    color: var(--text-primary);
                    margin: 0;
                }

                .tabular-nums {
                    font-variant-numeric: tabular-nums;
                }

                .mono {
                    font-family: 'JetBrains Mono', ui-monospace, SFMono-Regular, Menlo, monospace;
                }

                a:focus-visible,
                button:focus-visible,
                input:focus-visible {
                    outline: 2px solid var(--brand-navy);
                    outline-offset: 2px;
                }

                .container {
                    max-width: 1200px;
                    margin: 0 auto;
                    padding: 0 24px;
                }

                /* Header / Navbar (72px sticky) */
                .site-header {
                    position: sticky;
                    top: 0;
                    z-index: 50;
                    height: 72px;
                    background: rgba(255, 255, 255, 0.92);
                    backdrop-filter: blur(10px);
                    border-bottom: 1px solid var(--border-subtle);
                    box-shadow: var(--shadow-nav);
                }

                .header-inner {
                    height: 100%;
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    gap: 24px;
                }

                .brand {
                    display: inline-flex;
                    align-items: center;
                    gap: 10px;
                    text-decoration: none;
                    color: var(--brand-navy);
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-weight: 800;
                    font-size: 1.125rem;
                    letter-spacing: -0.02em;
                    flex-shrink: 0;
                }

                .brand-mark {
                    width: 36px;
                    height: 36px;
                    border-radius: var(--radius-md);
                    background: var(--brand-navy);
                    color: #FFFFFF;
                    display: inline-flex;
                    align-items: center;
                    justify-content: center;
                    box-shadow: inset 0 -2px 0 rgba(217, 119, 6, 0.85);
                }

                .main-nav {
                    display: flex;
                    align-items: center;
                    gap: 4px;
                }

                .nav-link {
                    color: var(--text-secondary);
                    text-decoration: none;
                    font-size: 0.9375rem;
                    font-weight: 500;
                    padding: 8px 12px;
                    border-radius: var(--radius-md);
                    transition: color 160ms ease, background-color 160ms ease;
                    white-space: nowrap;
                }

                .nav-link:hover {
                    color: var(--brand-navy);
                    background-color: var(--brand-navy-tint);
                }

                .nav-link.active {
                    color: var(--brand-navy);
                    background-color: var(--brand-navy-tint);
                    font-weight: 600;
                }

                .header-actions {
                    display: flex;
                    align-items: center;
                    gap: 10px;
                    flex-shrink: 0;
                }

                /* Buttons (8px radius) */
                .btn {
                    display: inline-flex;
                    align-items: center;
                    justify-content: center;
                    gap: 8px;
                    min-height: 40px;
                    padding: 10px 18px;
                    border-radius: var(--radius-md);
                    font-family: 'Inter', sans-serif;
                    font-size: 0.875rem;
                    font-weight: 600;
                    line-height: 1.2;
                    text-decoration: none;
                    cursor: pointer;
                    border: 1px solid transparent;
                    transition: all 160ms ease;
                }

                .btn-gold {
                    background-color: var(--accent-gold);
                    color: #FFFFFF;
                }

                .btn-gold:hover {
                    background-color: var(--accent-gold-hover);
                    transform: translateY(-1px);
                    box-shadow: 0 6px 16px rgba(217, 119, 6, 0.24);
                }

                .btn-navy {
                    background-color: var(--brand-navy);
                    color: #FFFFFF;
                }

                .btn-navy:hover {
                    background-color: var(--brand-navy-hover);
                    transform: translateY(-1px);
                    box-shadow: 0 6px 16px rgba(30, 58, 138, 0.2);
                }

                .btn-outline {
                    background-color: var(--bg-surface);
                    color: var(--text-primary);
                    border-color: var(--border-strong);
                }

                .btn-outline:hover {
                    background-color: var(--bg-canvas);
                    border-color: var(--brand-navy);
                    color: var(--brand-navy);
                }

                .btn-block {
                    width: 100%;
                }

                /* Shared Badges & Sections */
                .eyebrow-badge {
                    display: inline-flex;
                    align-items: center;
                    gap: 8px;
                    padding: 4px 12px;
                    border-radius: var(--radius-pill);
                    background-color: var(--brand-navy-tint);
                    border: 1px solid #DBEAFE;
                    color: var(--brand-navy);
                    font-size: 0.75rem;
                    font-weight: 600;
                    letter-spacing: 0.03em;
                    text-transform: uppercase;
                    margin-bottom: 20px;
                }

                .eyebrow-dot {
                    width: 7px;
                    height: 7px;
                    border-radius: 50%;
                    background-color: var(--accent-gold);
                }

                .page-section {
                    padding: 64px 0;
                }

                .page-section.surface-band {
                    background-color: var(--bg-surface);
                    border-top: 1px solid var(--border-subtle);
                    border-bottom: 1px solid var(--border-subtle);
                }

                .section-header {
                    display: flex;
                    align-items: flex-end;
                    justify-content: space-between;
                    gap: 20px;
                    margin-bottom: 32px;
                    flex-wrap: wrap;
                }

                .section-kicker {
                    display: inline-block;
                    font-size: 0.75rem;
                    font-weight: 600;
                    letter-spacing: 0.04em;
                    text-transform: uppercase;
                    color: var(--accent-gold);
                    margin-bottom: 6px;
                }

                .section-title {
                    font-size: 1.625rem;
                    font-weight: 700;
                    line-height: 1.25;
                    letter-spacing: -0.02em;
                }

                .section-desc {
                    color: var(--text-secondary);
                    font-size: 0.95rem;
                    margin: 6px 0 0;
                    max-width: 600px;
                }

                .badge-row {
                    display: flex;
                    flex-wrap: wrap;
                    gap: 8px;
                }

                .badge {
                    display: inline-flex;
                    align-items: center;
                    padding: 4px 10px;
                    border-radius: var(--radius-pill);
                    font-size: 0.75rem;
                    font-weight: 600;
                    line-height: 1.2;
                }

                .badge-navy {
                    background-color: var(--brand-navy-tint);
                    color: var(--brand-navy);
                    border: 1px solid #DBEAFE;
                }

                .badge-gold {
                    background-color: var(--accent-gold-tint);
                    color: #92400E;
                }

                /* Footer */
                .site-footer {
                    background-color: var(--brand-navy-deep);
                    color: #CBD5E1;
                    padding: 40px 0;
                    border-top: 3px solid var(--accent-gold);
                }

                .footer-inner {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    gap: 24px;
                    flex-wrap: wrap;
                }

                .footer-brand {
                    color: #FFFFFF;
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-weight: 700;
                    font-size: 1rem;
                    margin-bottom: 4px;
                }

                .footer-note {
                    font-size: 0.875rem;
                    color: #94A3B8;
                }

                .footer-links {
                    display: flex;
                    flex-wrap: wrap;
                    gap: 20px;
                }

                .footer-links a {
                    color: #E2E8F0;
                    text-decoration: none;
                    font-size: 0.875rem;
                    font-weight: 500;
                }

                .footer-links a:hover {
                    color: #FCD34D;
                }

                @media (max-width: 1023px) {
                    .main-nav {
                        display: none;
                    }
                }

                @media (max-width: 767px) {
                    .page-section {
                        padding: 44px 0;
                    }
                    .header-actions .btn-outline {
                        display: none;
                    }
                }

                {{extraCss}}
            </style>
        </head>
        <body>
            <header class="site-header">
                <div class="container header-inner">
                    <a href="/" class="brand">
                        <span class="brand-mark" aria-hidden="true">
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round">
                                <path d="M22 10v6M2 10l10-5 10 5-10 5z"/>
                                <path d="M6 12v5c3 3 9 3 12 0v-5"/>
                            </svg>
                        </span>
                        <span>Alumni Network</span>
                    </a>

                    <nav class="main-nav" aria-label="Ana Navigasyon">
                        <a href="/" class="{{NavClass("/")}}">Ana Sayfa</a>
                        <a href="/users" class="{{NavClass("/users")}}">Mezunlar</a>
                        <a href="/jobs" class="{{NavClass("/jobs")}}">İş İlanları</a>
                        <a href="/applications" class="{{NavClass("/applications")}}">Başvurular</a>
                        <a href="/announcements" class="{{NavClass("/announcements")}}">Duyurular</a>
                        <a href="/about" class="{{NavClass("/about")}}">Hakkında</a>
                    </nav>

                    <div class="header-actions">
                        <a href="/api/swagger" class="btn btn-outline">API Docs</a>
                        <a href="/jobs" class="btn btn-gold">Fırsatları Keşfet</a>
                    </div>
                </div>
            </header>

            <main>
                {{mainContent}}
            </main>

            <footer class="site-footer">
                <div class="container footer-inner">
                    <div>
                        <div class="footer-brand">Alumni Tracking System</div>
                        <div class="footer-note">Üniversite Kariyer Merkezi &amp; Mezunlar Ofisi • Akademik Lacivert &amp; Altın Tasarım Sistemi</div>
                    </div>
                    <nav class="footer-links" aria-label="Alt Menü">
                        <a href="/about">Hakkında</a>
                        <a href="/users">Mezunlar</a>
                        <a href="/jobs">İş İlanları</a>
                        <a href="/announcements">Duyurular</a>
                        <a href="/api/swagger">Swagger / API</a>
                        <a href="mailto:alumni@ogr.iu.edu.tr">İletişim</a>
                    </nav>
                </div>
            </footer>
        </body>
        </html>
        """;
    }

    public static string GetHomePageHtml()
    {
        const string homeCss = """
                .hero-section {
                    padding: 64px 0 56px;
                    background:
                        radial-gradient(ellipse 80% 60% at 85% 0%, rgba(30, 58, 138, 0.06), transparent 65%),
                        radial-gradient(ellipse 60% 50% at 10% 20%, rgba(217, 119, 6, 0.05), transparent 60%),
                        var(--bg-canvas);
                    border-bottom: 1px solid var(--border-subtle);
                }
                .hero-grid {
                    display: grid;
                    grid-template-columns: 1.15fr 0.85fr;
                    gap: 48px;
                    align-items: center;
                }
                .hero-title {
                    font-size: clamp(2rem, 4.2vw, 3.125rem);
                    font-weight: 800;
                    line-height: 1.12;
                    letter-spacing: -0.03em;
                    margin-bottom: 20px;
                }
                .hero-title .accent { color: var(--brand-navy); }
                .hero-lead {
                    font-size: 1.125rem;
                    color: var(--text-secondary);
                    line-height: 1.65;
                    max-width: 560px;
                    margin: 0 0 28px;
                }
                .search-form {
                    display: flex;
                    align-items: center;
                    background-color: var(--bg-surface);
                    border: 1px solid var(--border-strong);
                    border-radius: 10px;
                    padding: 6px;
                    box-shadow: var(--shadow-sm);
                    max-width: 580px;
                    transition: border-color 160ms ease, box-shadow 160ms ease;
                }
                .search-form:focus-within {
                    border-color: var(--brand-navy);
                    box-shadow: 0 0 0 3px rgba(30, 58, 138, 0.14);
                }
                .search-icon {
                    color: var(--text-muted);
                    margin-left: 12px;
                    display: inline-flex;
                    flex-shrink: 0;
                }
                .search-form input {
                    flex: 1;
                    min-width: 0;
                    border: none;
                    background: transparent;
                    padding: 10px 12px;
                    font-family: 'Inter', sans-serif;
                    font-size: 0.95rem;
                    color: var(--text-primary);
                    outline: none;
                }
                .search-form input::placeholder { color: #94A3B8; }
                .hero-links {
                    display: flex;
                    flex-wrap: wrap;
                    align-items: center;
                    gap: 16px;
                    margin-top: 20px;
                }
                .quick-links {
                    font-size: 0.875rem;
                    color: var(--text-muted);
                    display: flex;
                    flex-wrap: wrap;
                    align-items: center;
                    gap: 8px;
                }
                .quick-chip {
                    color: var(--brand-navy);
                    background-color: var(--bg-surface);
                    border: 1px solid var(--border-subtle);
                    padding: 4px 10px;
                    border-radius: var(--radius-pill);
                    font-size: 0.75rem;
                    font-weight: 600;
                    text-decoration: none;
                    transition: border-color 160ms ease, background-color 160ms ease;
                }
                .quick-chip:hover {
                    border-color: var(--brand-navy);
                    background-color: var(--brand-navy-tint);
                }
                .summary-panel {
                    background-color: var(--bg-surface);
                    border: 1px solid var(--border-subtle);
                    border-radius: var(--radius-lg);
                    box-shadow: var(--shadow-md);
                    overflow: hidden;
                }
                .summary-header {
                    background-color: var(--brand-navy);
                    color: #FFFFFF;
                    padding: 16px 22px;
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    gap: 12px;
                }
                .summary-header-title {
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-size: 1rem;
                    font-weight: 700;
                    color: #FFFFFF;
                }
                .summary-header-sub {
                    font-size: 0.75rem;
                    color: #DBEAFE;
                    margin-top: 2px;
                }
                .verified-pill {
                    background-color: rgba(255, 255, 255, 0.14);
                    border: 1px solid rgba(255, 255, 255, 0.24);
                    color: #FEF3C7;
                    font-size: 0.75rem;
                    font-weight: 600;
                    padding: 4px 10px;
                    border-radius: var(--radius-pill);
                    white-space: nowrap;
                }
                .summary-body { padding: 20px; }
                .kpi-grid {
                    display: grid;
                    grid-template-columns: repeat(2, 1fr);
                    gap: 12px;
                }
                .kpi-box {
                    background-color: var(--bg-canvas);
                    border: 1px solid var(--border-subtle);
                    border-radius: var(--radius-md);
                    padding: 16px;
                }
                .kpi-value {
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-size: 1.625rem;
                    font-weight: 800;
                    color: var(--brand-navy);
                    line-height: 1.15;
                }
                .kpi-value.gold { color: var(--accent-gold); }
                .kpi-label {
                    font-size: 0.8125rem;
                    color: var(--text-muted);
                    margin-top: 4px;
                }
                .summary-footer {
                    margin-top: 16px;
                    padding-top: 14px;
                    border-top: 1px solid var(--border-subtle);
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    font-size: 0.8125rem;
                    color: var(--text-secondary);
                }
                .status-dot {
                    display: inline-flex;
                    align-items: center;
                    gap: 6px;
                    color: var(--status-success);
                    font-weight: 600;
                }
                .status-dot::before {
                    content: "";
                    width: 8px;
                    height: 8px;
                    border-radius: 50%;
                    background-color: var(--status-success);
                }
                .alumni-grid {
                    display: grid;
                    grid-template-columns: repeat(3, 1fr);
                    gap: 24px;
                }
                .alumni-card {
                    background-color: var(--bg-surface);
                    border: 1px solid var(--border-subtle);
                    border-radius: var(--radius-lg);
                    box-shadow: var(--shadow-sm);
                    overflow: hidden;
                    display: flex;
                    flex-direction: column;
                    transition: transform 160ms ease, box-shadow 160ms ease, border-color 160ms ease;
                }
                .alumni-card:hover {
                    transform: translateY(-2px);
                    box-shadow: var(--shadow-md);
                    border-color: var(--border-strong);
                }
                .alumni-card-banner {
                    background-color: var(--brand-navy);
                    padding: 16px 20px;
                    display: flex;
                    align-items: center;
                    gap: 12px;
                }
                .alumni-avatar {
                    width: 42px;
                    height: 42px;
                    border-radius: 50%;
                    background-color: var(--bg-surface);
                    color: var(--brand-navy);
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-weight: 800;
                    font-size: 0.95rem;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    flex-shrink: 0;
                    border: 2px solid var(--accent-gold);
                }
                .alumni-banner-name {
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-size: 1rem;
                    font-weight: 700;
                    color: #FFFFFF;
                    line-height: 1.3;
                }
                .alumni-banner-role { font-size: 0.8125rem; color: #DBEAFE; }
                .alumni-card-body {
                    padding: 20px;
                    display: flex;
                    flex-direction: column;
                    flex: 1;
                }
                .alumni-org {
                    font-weight: 600;
                    color: var(--text-primary);
                    font-size: 0.95rem;
                    margin-bottom: 12px;
                }
                .alumni-meta-list {
                    list-style: none;
                    margin: 0 0 16px;
                    padding: 0;
                    display: grid;
                    gap: 6px;
                    font-size: 0.875rem;
                    color: var(--text-secondary);
                }
                .alumni-meta-list li strong { color: var(--text-primary); font-weight: 600; }
                .alumni-card .badge-row { margin-bottom: 20px; }
                .alumni-card-footer { margin-top: auto; }
                .modules-grid {
                    display: grid;
                    grid-template-columns: repeat(4, 1fr);
                    gap: 20px;
                }
                .module-card {
                    background-color: var(--bg-canvas);
                    border: 1px solid var(--border-subtle);
                    border-radius: var(--radius-lg);
                    padding: 24px;
                    text-decoration: none;
                    color: inherit;
                    display: flex;
                    flex-direction: column;
                    transition: transform 160ms ease, box-shadow 160ms ease, border-color 160ms ease, background-color 160ms ease;
                }
                .module-card:hover {
                    background-color: var(--bg-surface);
                    border-color: var(--brand-navy);
                    box-shadow: var(--shadow-md);
                    transform: translateY(-2px);
                }
                .module-icon {
                    width: 44px;
                    height: 44px;
                    border-radius: var(--radius-md);
                    background-color: var(--brand-navy-tint);
                    color: var(--brand-navy);
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin-bottom: 16px;
                }
                .module-card h3 { font-size: 1.0625rem; font-weight: 700; margin-bottom: 8px; }
                .module-card p {
                    font-size: 0.875rem;
                    color: var(--text-secondary);
                    margin: 0 0 16px;
                    line-height: 1.55;
                    flex: 1;
                }
                .module-route {
                    font-size: 0.8125rem;
                    font-weight: 600;
                    color: var(--brand-navy);
                    display: inline-flex;
                    align-items: center;
                    justify-content: space-between;
                    padding-top: 12px;
                    border-top: 1px solid var(--border-subtle);
                }
                @media (max-width: 1023px) {
                    .hero-grid { grid-template-columns: 1fr; gap: 32px; }
                    .alumni-grid, .modules-grid { grid-template-columns: repeat(2, 1fr); }
                }
                @media (max-width: 767px) {
                    .hero-section { padding: 40px 0 44px; }
                    .alumni-grid, .modules-grid { grid-template-columns: 1fr; }
                    .search-form { flex-direction: column; align-items: stretch; gap: 8px; padding: 10px; }
                    .search-icon { display: none; }
                    .search-form .btn { min-height: 44px; width: 100%; }
                }
                """;

        const string homeMain = """
                <section class="hero-section">
                    <div class="container hero-grid">
                        <div>
                            <span class="eyebrow-badge">
                                <span class="eyebrow-dot" aria-hidden="true"></span>
                                Kariyer Merkezi &amp; Mezunlar Ofisi
                            </span>
                            <h1 class="hero-title">
                                Köklü Akademik Miras, <span class="accent">Güçlü Kariyer Ağı.</span>
                            </h1>
                            <p class="hero-lead">
                                Üniversitemiz mezunlarının kariyer yolculuklarını takip edin, sektör profesyonelleriyle bağlantı kurun ve güncel iş fırsatlarına tek merkezden erişin.
                            </p>

                            <form class="search-form" action="/users" method="get" role="search">
                                <span class="search-icon" aria-hidden="true">
                                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                        <circle cx="11" cy="11" r="8"/>
                                        <path d="m21 21-4.3-4.3"/>
                                    </svg>
                                </span>
                                <input type="search" name="q" placeholder="Mezun adı, bölüm, şirket veya mezuniyet yılı ara..." aria-label="Mezun ara">
                                <button type="submit" class="btn btn-gold">Mezun Ara</button>
                            </form>

                            <div class="hero-links">
                                <div class="quick-links">
                                    <span>Hızlı Geçiş:</span>
                                    <a href="/users" class="quick-chip">Mezun Dizini</a>
                                    <a href="/jobs" class="quick-chip">Açık İlanlar</a>
                                    <a href="/announcements" class="quick-chip">Duyurular</a>
                                </div>
                            </div>
                        </div>

                        <aside class="summary-panel" aria-label="Platform Özet İstatistikleri">
                            <div class="summary-header">
                                <div>
                                    <div class="summary-header-title">Kurumsal Mezun Ağı Özeti</div>
                                    <div class="summary-header-sub">Doğrulanmış Kariyer &amp; İstihdam Verileri</div>
                                </div>
                                <span class="verified-pill">Approved Network</span>
                            </div>
                            <div class="summary-body">
                                <div class="kpi-grid">
                                    <div class="kpi-box">
                                        <div class="kpi-value tabular-nums">4,820+</div>
                                        <div class="kpi-label">Onaylı Mezun Profili</div>
                                    </div>
                                    <div class="kpi-box">
                                        <div class="kpi-value gold tabular-nums">310+</div>
                                        <div class="kpi-label">Partner Şirket &amp; Kurum</div>
                                    </div>
                                    <div class="kpi-box">
                                        <div class="kpi-value tabular-nums">%92.4</div>
                                        <div class="kpi-label">İlk 6 Ayda İstihdam</div>
                                    </div>
                                    <div class="kpi-box">
                                        <div class="kpi-value tabular-nums">128</div>
                                        <div class="kpi-label">Aktif İş &amp; Staj İlanı</div>
                                    </div>
                                </div>
                                <div class="summary-footer">
                                    <span class="status-dot">Sistem ve API Çevrimiçi</span>
                                    <a href="/api/health" class="mono" style="color: var(--brand-navy); text-decoration: none; font-weight: 600;">/api/health</a>
                                </div>
                            </div>
                        </aside>
                    </div>
                </section>

                <section class="page-section">
                    <div class="container">
                        <div class="section-header">
                            <div>
                                <span class="section-kicker">Seçkin Mezun Profilleri</span>
                                <h2 class="section-title">Öne Çıkan Mezunlarımız</h2>
                                <p class="section-desc">Farklı sektörlerde öncü roller üstlenen onaylı mezunlarımızla tanışın ve profesyonel ağınızı büyütün.</p>
                            </div>
                            <a href="/users" class="btn btn-outline">Tüm Mezunları Görüntüle →</a>
                        </div>

                        <div class="alumni-grid">
                            <article class="alumni-card">
                                <div class="alumni-card-banner">
                                    <div class="alumni-avatar" aria-hidden="true">ZK</div>
                                    <div>
                                        <div class="alumni-banner-name">Dr. Zeynep Kaya</div>
                                        <div class="alumni-banner-role">Mezun Profili</div>
                                    </div>
                                </div>
                                <div class="alumni-card-body">
                                    <div class="alumni-org">Bilgisayar Mühendisliği • Principal Architect</div>
                                    <ul class="alumni-meta-list">
                                        <li class="tabular-nums"><strong>Mezuniyet:</strong> 2011</li>
                                        <li><strong>Sektör:</strong> Yazılım &amp; Bulut Sistemleri</li>
                                    </ul>
                                    <div class="badge-row">
                                        <span class="badge badge-navy">Doktora Mezunu</span>
                                        <span class="badge badge-gold">Mentor</span>
                                    </div>
                                    <div class="alumni-card-footer">
                                        <a href="/users" class="btn btn-gold btn-block">Bağlantı Kur</a>
                                    </div>
                                </div>
                            </article>

                            <article class="alumni-card">
                                <div class="alumni-card-banner">
                                    <div class="alumni-avatar" aria-hidden="true">AY</div>
                                    <div>
                                        <div class="alumni-banner-name">Ahmet Yılmaz</div>
                                        <div class="alumni-banner-role">Mezun Profili</div>
                                    </div>
                                </div>
                                <div class="alumni-card-body">
                                    <div class="alumni-org">Bilgisayar Mühendisliği • Senior Software Engineer</div>
                                    <ul class="alumni-meta-list">
                                        <li class="tabular-nums"><strong>Mezuniyet:</strong> 2019</li>
                                        <li><strong>Sektör:</strong> Kurumsal Teknoloji (@ TechCorp)</li>
                                    </ul>
                                    <div class="badge-row">
                                        <span class="badge badge-navy">Mühendislik</span>
                                        <span class="badge badge-gold">İlan Veren</span>
                                    </div>
                                    <div class="alumni-card-footer">
                                        <a href="/users" class="btn btn-gold btn-block">Bağlantı Kur</a>
                                    </div>
                                </div>
                            </article>

                            <article class="alumni-card">
                                <div class="alumni-card-banner">
                                    <div class="alumni-avatar" aria-hidden="true">AD</div>
                                    <div>
                                        <div class="alumni-banner-name">Ayşe Demir</div>
                                        <div class="alumni-banner-role">Mezun Profili</div>
                                    </div>
                                </div>
                                <div class="alumni-card-body">
                                    <div class="alumni-org">Endüstri Mühendisliği • Lead Product Manager</div>
                                    <ul class="alumni-meta-list">
                                        <li class="tabular-nums"><strong>Mezuniyet:</strong> 2021</li>
                                        <li><strong>Sektör:</strong> Ürün Yönetimi (@ InnovateAŞ)</li>
                                    </ul>
                                    <div class="badge-row">
                                        <span class="badge badge-navy">SaaS &amp; Strateji</span>
                                        <span class="badge badge-gold">Kariyer Elçisi</span>
                                    </div>
                                    <div class="alumni-card-footer">
                                        <a href="/users" class="btn btn-gold btn-block">Bağlantı Kur</a>
                                    </div>
                                </div>
                            </article>
                        </div>
                    </div>
                </section>

                <section class="page-section surface-band">
                    <div class="container">
                        <div class="section-header">
                            <div>
                                <span class="section-kicker">Platform Modülleri</span>
                                <h2 class="section-title">Tek Çatı Altında Mezun Ekosistemi</h2>
                                <p class="section-desc">Kayıtlı mezun profillerinden iş ilanlarına, başvuru süreçlerinden duyurulara kadar tüm modüllere doğrudan erişin.</p>
                            </div>
                        </div>

                        <div class="modules-grid">
                            <a href="/users" class="module-card">
                                <div class="module-icon" aria-hidden="true">
                                    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                        <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/>
                                        <circle cx="9" cy="7" r="4"/>
                                        <path d="M22 21v-2a4 4 0 0 0-3-3.87"/>
                                        <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
                                    </svg>
                                </div>
                                <h3>Mezun &amp; Kullanıcı Dizini</h3>
                                <p>Mezun profillerini görüntüleyin, yeni mezun kaydı oluşturun ve mezunların açtığı ilanları inceleyin.</p>
                                <div class="module-route">
                                    <span class="mono">/users</span>
                                    <span>Git →</span>
                                </div>
                            </a>

                            <a href="/jobs" class="module-card">
                                <div class="module-icon" aria-hidden="true">
                                    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                        <rect width="20" height="14" x="2" y="7" rx="2"/>
                                        <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"/>
                                    </svg>
                                </div>
                                <h3>İş ve Staj İlanları</h3>
                                <p>Mezunlar ve partner şirketler tarafından açılan güncel pozisyonları keşfedin veya yeni ilan ekleyin.</p>
                                <div class="module-route">
                                    <span class="mono">/jobs</span>
                                    <span>Git →</span>
                                </div>
                            </a>

                            <a href="/applications" class="module-card">
                                <div class="module-icon" aria-hidden="true">
                                    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                        <path d="M9 11l3 3L22 4"/>
                                        <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"/>
                                    </svg>
                                </div>
                                <h3>Başvuru Yönetimi</h3>
                                <p>İlan başvurularını oluşturun ve aday değerlendirme aşamalarını (Applied → Accepted) canlı yönetin.</p>
                                <div class="module-route">
                                    <span class="mono">/applications</span>
                                    <span>Git →</span>
                                </div>
                            </a>

                            <a href="/announcements" class="module-card">
                                <div class="module-icon" aria-hidden="true">
                                    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                        <path d="m3 11 18-5v12L3 14v-3z"/>
                                        <path d="M11.6 16.8a3 3 0 1 1-5.8-1.6"/>
                                    </svg>
                                </div>
                                <h3>Duyurular &amp; Etkinlikler</h3>
                                <p>Kariyer Merkezi buluşmaları, mezuniyet törenleri ve sektörel seminer duyurularını takip edin.</p>
                                <div class="module-route">
                                    <span class="mono">/announcements</span>
                                    <span>Git →</span>
                                </div>
                            </a>
                        </div>
                    </div>
                </section>
                """;

        return RenderPageShell("Alumni Tracking System — Akademik Mezun Ağı", "/", homeCss, homeMain);
    }

    public static string GetAboutPageHtml()
    {
        const string aboutCss = """
                .about-hero {
                    padding: 64px 0 56px;
                    background:
                        radial-gradient(ellipse 75% 60% at 50% 0%, rgba(30, 58, 138, 0.06), transparent 70%),
                        var(--bg-canvas);
                    border-bottom: 1px solid var(--border-subtle);
                    text-align: center;
                }
                .about-hero h1 {
                    font-size: clamp(2rem, 3.8vw, 2.875rem);
                    font-weight: 800;
                    line-height: 1.15;
                    letter-spacing: -0.03em;
                    margin-bottom: 16px;
                }
                .about-hero p {
                    font-size: 1.125rem;
                    color: var(--text-secondary);
                    max-width: 680px;
                    margin: 0 auto 28px;
                    line-height: 1.65;
                }
                .about-hero-actions {
                    display: flex;
                    justify-content: center;
                    gap: 12px;
                    flex-wrap: wrap;
                }
                .pillars-grid {
                    display: grid;
                    grid-template-columns: repeat(3, 1fr);
                    gap: 24px;
                }
                .pillar-card {
                    background-color: var(--bg-surface);
                    border: 1px solid var(--border-subtle);
                    border-radius: var(--radius-lg);
                    padding: 28px 24px;
                    box-shadow: var(--shadow-sm);
                    transition: transform 160ms ease, box-shadow 160ms ease, border-color 160ms ease;
                }
                .pillar-card:hover {
                    transform: translateY(-2px);
                    box-shadow: var(--shadow-md);
                    border-color: var(--border-strong);
                }
                .pillar-icon {
                    width: 48px;
                    height: 48px;
                    border-radius: var(--radius-md);
                    background-color: var(--brand-navy-tint);
                    color: var(--brand-navy);
                    display: inline-flex;
                    align-items: center;
                    justify-content: center;
                    margin-bottom: 18px;
                }
                .pillar-card h3 {
                    font-size: 1.125rem;
                    font-weight: 700;
                    margin-bottom: 10px;
                    color: var(--brand-navy);
                }
                .pillar-card p {
                    color: var(--text-secondary);
                    font-size: 0.9375rem;
                    margin: 0;
                    line-height: 1.6;
                }
                .principles-grid {
                    display: grid;
                    grid-template-columns: repeat(2, 1fr);
                    gap: 24px;
                }
                .principle-item {
                    background-color: var(--bg-canvas);
                    border: 1px solid var(--border-subtle);
                    border-radius: var(--radius-lg);
                    padding: 24px;
                    display: flex;
                    gap: 16px;
                    align-items: flex-start;
                }
                .principle-step {
                    width: 34px;
                    height: 34px;
                    border-radius: var(--radius-md);
                    background-color: var(--brand-navy);
                    color: #FFFFFF;
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-weight: 700;
                    font-size: 0.875rem;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    flex-shrink: 0;
                }
                .principle-item h3 {
                    font-size: 1rem;
                    font-weight: 700;
                    margin-bottom: 6px;
                }
                .principle-item p {
                    margin: 0;
                    font-size: 0.9rem;
                    color: var(--text-secondary);
                }
                .contact-card {
                    max-width: 820px;
                    margin: 0 auto;
                    background-color: var(--bg-surface);
                    border: 1px solid var(--border-subtle);
                    border-radius: var(--radius-lg);
                    box-shadow: var(--shadow-md);
                    overflow: hidden;
                }
                .contact-card-header {
                    background-color: var(--brand-navy);
                    color: #FFFFFF;
                    padding: 20px 28px;
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    gap: 16px;
                    flex-wrap: wrap;
                }
                .contact-card-header h2 {
                    color: #FFFFFF;
                    font-size: 1.25rem;
                    font-weight: 700;
                }
                .contact-card-body {
                    padding: 28px;
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    gap: 24px;
                    flex-wrap: wrap;
                }
                .contact-card-body p {
                    margin: 0;
                    color: var(--text-secondary);
                    max-width: 480px;
                    font-size: 0.95rem;
                }
                .contact-actions {
                    display: flex;
                    gap: 12px;
                    flex-wrap: wrap;
                }
                @media (max-width: 1023px) {
                    .pillars-grid { grid-template-columns: repeat(2, 1fr); }
                }
                @media (max-width: 767px) {
                    .about-hero { padding: 44px 0; }
                    .pillars-grid, .principles-grid { grid-template-columns: 1fr; }
                }
                """;

        const string aboutMain = """
                <section class="about-hero">
                    <div class="container">
                        <span class="eyebrow-badge">
                            <span class="eyebrow-dot" aria-hidden="true"></span>
                            Platform Misyonu &amp; Kurumsal Vizyon
                        </span>
                        <h1>Mezun Takip Platformu Hakkında</h1>
                        <p>
                            Üniversitemiz mezunları ile mevcut öğrenciler arasındaki bağı güçlendirmeyi, kariyer yolculuklarını ilişkisel olarak görünür kılmayı ve köklü bir akademik iletişim ağı inşa etmeyi amaçlıyoruz.
                        </p>
                        <div class="about-hero-actions">
                            <a href="/users" class="btn btn-gold">Mezun Dizinini Keşfet</a>
                            <a href="/jobs" class="btn btn-outline">Açık İlanları Gör</a>
                        </div>
                    </div>
                </section>

                <section class="page-section">
                    <div class="container">
                        <div class="section-header">
                            <div>
                                <span class="section-kicker">Temel Değer Önerisi</span>
                                <h2 class="section-title">Neden Alumni Tracking System?</h2>
                                <p class="section-desc">Akademi ile iş dünyasını güvenilir, doğrulanmış ve sürdürülebilir bir veri mimarisi altında buluşturuyoruz.</p>
                            </div>
                        </div>

                        <div class="pillars-grid">
                            <article class="pillar-card">
                                <div class="pillar-icon" aria-hidden="true">
                                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                        <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/>
                                        <circle cx="9" cy="7" r="4"/>
                                        <path d="M22 21v-2a4 4 0 0 0-3-3.87"/>
                                        <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
                                    </svg>
                                </div>
                                <h3>Mezun İletişim Ağı</h3>
                                <p>Yıllara, fakültelere ve bölümlere göre mezunların güncel kariyer, unvan ve kurum bilgilerine erişim sağlayarak profesyonel ağınızı genişletin.</p>
                            </article>

                            <article class="pillar-card">
                                <div class="pillar-icon" aria-hidden="true">
                                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                        <rect width="20" height="14" x="2" y="7" rx="2"/>
                                        <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"/>
                                    </svg>
                                </div>
                                <h3>Kariyer ve Mentorluk</h3>
                                <p>Yeni mezun ve öğrencilere yönelik staj, tam zamanlı iş fırsatları ve sektörel mentorluk kanallarıyla kariyer planlamasına doğrudan yön verin.</p>
                            </article>

                            <article class="pillar-card">
                                <div class="pillar-icon" aria-hidden="true">
                                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                                        <path d="M3 21h18"/>
                                        <path d="M5 21V7l8-4v18"/>
                                        <path d="M19 21V11l-6-4"/>
                                        <path d="M9 9v.01"/><path d="M9 12v.01"/><path d="M9 15v.01"/>
                                    </svg>
                                </div>
                                <h3>Üniversite &amp; Sanayi Köprüsü</h3>
                                <p>Mezunların görev aldığı kurumlarla üniversite arasındaki iş birliklerini güçlendirerek istihdam analitiği ve kurumsal ortaklıkları destekliyoruz.</p>
                            </article>
                        </div>
                    </div>
                </section>

                <section class="page-section surface-band">
                    <div class="container">
                        <div class="section-header">
                            <div>
                                <span class="section-kicker">Kurumsal İş Kuralları</span>
                                <h2 class="section-title">Platform Güvenilirlik İlkeleri</h2>
                                <p class="section-desc">Sistemdeki tüm veriler PostgreSQL ilişkisel bütünlüğü ve kurumsal doğrulama kuralları çerçevesinde yönetilir.</p>
                            </div>
                        </div>

                        <div class="principles-grid">
                            <div class="principle-item">
                                <span class="principle-step tabular-nums">01</span>
                                <div>
                                    <h3>Doğrulanmış Mezun Profilleri</h3>
                                    <p>Genel aramalarda ve istihdam istatistiklerinde yalnızca Kariyer Merkezi onayından geçmiş (<span class="mono">Approved</span>) mezun profilleri yer alır.</p>
                                </div>
                            </div>

                            <div class="principle-item">
                                <span class="principle-step tabular-nums">02</span>
                                <div>
                                    <h3>Aktif İstihdam Bütünlüğü</h3>
                                    <p>Mezunların geçmiş deneyimleri kronolojik olarak saklanırken aynı anda yalnızca bir adet güncel aktif pozisyon (<span class="mono">IsCurrent</span>) tutulur.</p>
                                </div>
                            </div>

                            <div class="principle-item">
                                <span class="principle-step tabular-nums">03</span>
                                <div>
                                    <h3>Tekil Başvuru Güvencesi</h3>
                                    <p>İş ve staj ilanlarına yapılan başvurularda mükerrer kayıtlar veritabanı seviyesinde engellenir; tüm aşamalar şeffaf biçimde izlenir.</p>
                                </div>
                            </div>

                            <div class="principle-item">
                                <span class="principle-step tabular-nums">04</span>
                                <div>
                                    <h3>Dağıtılmış MVC &amp; Açık API</h3>
                                    <p>Hem Razor MVC ekranları hem de <span class="mono">/api/swagger</span> üzerinden erişilen RESTful uç noktalar aynı servis katmanını paylaşır.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>

                <section class="page-section">
                    <div class="container">
                        <div class="contact-card">
                            <div class="contact-card-header">
                                <h2>Kariyer Merkezi &amp; Mezunlar Ofisi İletişim</h2>
                                <span class="badge badge-gold">Resmî Destek Hattı</span>
                            </div>
                            <div class="contact-card-body">
                                <p>
                                    Mezun profil doğrulamaları, kurumsal iş ilanı ortaklıkları veya platform geri bildirimleriniz için Mezunlar Ofisi ile doğrudan iletişime geçebilirsiniz.
                                </p>
                                <div class="contact-actions">
                                    <a href="mailto:alumni@ogr.iu.edu.tr" class="btn btn-gold">alumni@ogr.iu.edu.tr</a>
                                    <a href="/" class="btn btn-outline">Ana Sayfaya Dön</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
                """;

        return RenderPageShell("Hakkında — Alumni Tracking System", "/about", aboutCss, aboutMain);
    }

    public static string GetNotFoundPageHtml(string? requestedPath = null, string? reason = null)
    {
        var displayPath = System.Net.WebUtility.HtmlEncode(
            string.IsNullOrWhiteSpace(requestedPath) ? "Bilinmeyen Rota" : requestedPath);
        var displayReason = System.Net.WebUtility.HtmlEncode(
            string.IsNullOrWhiteSpace(reason)
                ? "Aradığınız sayfa silinmiş, adı değiştirilmiş veya geçici olarak kullanım dışı kalmış olabilir."
                : reason);

        const string notFoundCss = """
                .not-found-wrap {
                    padding: 64px 0;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }
                .error-card {
                    background-color: var(--bg-surface);
                    max-width: 640px;
                    width: 100%;
                    border-radius: var(--radius-lg);
                    border: 1px solid var(--border-subtle);
                    box-shadow: var(--shadow-md);
                    overflow: hidden;
                }
                .error-card-banner {
                    background-color: var(--brand-navy);
                    color: #FFFFFF;
                    padding: 14px 24px;
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    gap: 12px;
                }
                .error-banner-title {
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-size: 0.875rem;
                    font-weight: 700;
                    letter-spacing: 0.02em;
                }
                .error-banner-code {
                    font-size: 0.75rem;
                    color: #FEF3C7;
                    background-color: rgba(255, 255, 255, 0.14);
                    padding: 3px 10px;
                    border-radius: var(--radius-pill);
                    font-weight: 600;
                }
                .error-card-body {
                    padding: 36px 32px;
                    text-align: center;
                }
                .error-code {
                    font-family: 'Plus Jakarta Sans', sans-serif;
                    font-size: clamp(3.5rem, 8vw, 5rem);
                    font-weight: 800;
                    line-height: 1;
                    color: var(--brand-navy);
                    letter-spacing: -0.04em;
                    margin-bottom: 12px;
                }
                .error-badge {
                    display: inline-flex;
                    align-items: center;
                    background-color: var(--status-danger-tint);
                    color: var(--status-danger);
                    border: 1px solid #FECACA;
                    padding: 4px 12px;
                    border-radius: var(--radius-pill);
                    font-size: 0.75rem;
                    font-weight: 600;
                    margin-bottom: 16px;
                }
                .error-card-body h1 {
                    font-size: 1.625rem;
                    font-weight: 700;
                    letter-spacing: -0.02em;
                    margin-bottom: 10px;
                }
                .error-lead {
                    color: var(--text-secondary);
                    font-size: 0.95rem;
                    margin: 0 0 24px;
                }
                .path-box {
                    background-color: var(--bg-surface-subtle);
                    border: 1px solid var(--border-subtle);
                    padding: 12px 16px;
                    border-radius: var(--radius-md);
                    margin-bottom: 14px;
                    font-size: 0.875rem;
                    color: var(--text-primary);
                    text-align: left;
                    word-break: break-all;
                }
                .reason-box {
                    background-color: var(--status-warning-tint);
                    border: 1px solid #FDE68A;
                    border-left: 4px solid var(--accent-gold);
                    padding: 12px 16px;
                    border-radius: var(--radius-md);
                    margin-bottom: 28px;
                    text-align: left;
                    font-size: 0.875rem;
                    color: #92400E;
                }
                .error-actions {
                    display: flex;
                    flex-wrap: wrap;
                    gap: 10px;
                    justify-content: center;
                }
                @media (max-width: 767px) {
                    .not-found-wrap { padding: 36px 0; }
                    .error-card-body { padding: 28px 20px; }
                }
                """;

        var notFoundMain = $$"""
                <section class="not-found-wrap">
                    <div class="container" style="display:flex; justify-content:center;">
                        <article class="error-card">
                            <div class="error-card-banner">
                                <span class="error-banner-title">Alumni Tracking System • Yönlendirme Koruyucusu</span>
                                <span class="error-banner-code mono">HTTP 404</span>
                            </div>
                            <div class="error-card-body">
                                <div class="error-code tabular-nums">404</div>
                                <div class="error-badge">Sayfa Bulunamadı (Not Found)</div>
                                <h1>Aradığınız Sayfaya Ulaşılamadı</h1>
                                <p class="error-lead">Ulaşmaya çalıştığınız web adresi sunucuda bulunamadı veya taşınmış olabilir.</p>

                                <div class="path-box">
                                    <strong>Hedef Rota:</strong> <span class="mono">{{displayPath}}</span>
                                </div>

                                <div class="reason-box">
                                    <strong>Gerekçe:</strong> {{displayReason}}
                                </div>

                                <div class="error-actions">
                                    <a href="/" class="btn btn-gold">Ana Sayfaya Dön</a>
                                    <a href="/users" class="btn btn-outline">Mezunlar</a>
                                    <a href="/jobs" class="btn btn-outline">İş İlanları</a>
                                    <a href="/announcements" class="btn btn-outline">Duyurular</a>
                                </div>
                            </div>
                        </article>
                    </div>
                </section>
                """;

        return RenderPageShell("404 — Sayfa Bulunamadı | Alumni Tracking System", "", notFoundCss, notFoundMain);
    }
}
