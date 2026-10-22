body {
    margin: 0;
    font-family: Arial, sans-serif;
    background: #f3f6f9;
    color: #1d1d1d;
}

.container {
    max-width: 1100px;
    margin: 0 auto;
    padding: 20px;
}

.topbar {
    background: #123a5a;
    color: white;
    padding: 14px 0;
}

.brand {
    color: white;
    text-decoration: none;
    font-weight: 700;
    font-size: 1.4rem;
}

.hero {
    background: linear-gradient(135deg, #0d3b66, #1d5f8d);
    color: white;
    padding: 40px 25px;
    border-radius: 14px;
    box-shadow: 0 10px 20px rgba(0,0,0,0.12);
}

.hero h1 {
    margin-top: 0;
    margin-bottom: 12px;
    font-size: 2.2rem;
}

.search-card {
    background: white;
    color: #111;
    border-radius: 12px;
    padding: 20px;
    margin-top: 20px;
    box-shadow: 0 10px 20px rgba(0,0,0,0.08);
}

.grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
    gap: 16px;
    margin-top: 10px;
}

.field {
    display: flex;
    flex-direction: column;
    gap: 6px;
}

.field label {
    font-weight: 600;
}

.field select,
.field input,
.field button {
    width: 100%;
    padding: 10px 12px;
    border-radius: 8px;
    border: 1px solid #c7d4dd;
    box-sizing: border-box;
}

.field button {
    background: #1a8f5a;
    color: white;
    border: none;
    cursor: pointer;
    font-weight: 700;
}

.info {
    margin-top: 14px;
    font-size: 0.95rem;
    color: #3b4c5d;
}

.results-header {
    margin-top: 32px;
    margin-bottom: 16px;
}

.section-box {
    background: white;
    border-radius: 12px;
    padding: 18px;
    box-shadow: 0 6px 14px rgba(0,0,0,0.06);
    margin-bottom: 24px;
}

.result-card {
    border: 1px solid #e2e6ea;
    border-radius: 10px;
    padding: 18px;
    margin-bottom: 16px;
    background: #fff;
}

.badge {
    display: inline-block;
    padding: 5px 10px;
    border-radius: 999px;
    font-size: 0.78rem;
    font-weight: 700;
    background: #dfeeff;
    color: #113c67;
    margin-bottom: 10px;
}

.badge.specialist {
    background: #dff8ea;
    color: #146d42;
}

.badge.general {
    background: #f2f2f2;
    color: #3d3d3d;
}

.empty-state {
    background: #eef4fa;
    border: 1px solid #d8e6f2;
    padding: 16px;
    border-radius: 10px;
}

.meta {
    font-size: 0.9rem;
    color: #455a6b;
    margin-bottom: 10px;
}

a {
    color: #0a5ea7;
}

@media (max-width: 768px) {
    .hero h1 {
        font-size: 1.7rem;
    }
}
