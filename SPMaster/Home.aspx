<%@ Page Title="" Language="C#" MasterPageFile="~/Main/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="SPMaster.Home" %>
<asp:Content ID="HomeHead" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link runat="server" href="~/Content/Home.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID ="HomeContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="home-page">

        <!-- HERO -->
        <section class="hero" aria-labelledby="hero-title">

            <div class="hero-copy">

                <h1 id="hero-title">
                    Make studying feel like your <span>superpower.</span>
                </h1>

                <p class="hero-description">
                    Practice smarter and walk into your SPM exams feeling ready. Your next big win starts with one tiny quiz.
                </p>

                <div class="hero-actions">
                    <a href="<%= ResolveUrl("~/Register.aspx") %>" class="button button-primary">
                        Start learning for free <span aria-hidden="true">→</span>
                    </a>

                    <a href="#how-it-works" class="button button-white">
                        <span aria-hidden="true">▷</span> See how it works
                    </a>
                </div>

                <div class="hero-note">
                    <span class="student-dots" aria-hidden="true">
                        <span></span>
                        <span></span>
                        <span></span>
                    </span>

                    <span>Six subjects. One place to practise.</span>
                </div>
            </div>

            <!-- Decorative artwork, created with CSS -->
            <div class="hero-art" aria-hidden="true">
                <div class="art-circle art-circle-orange"></div>
                <div class="art-circle art-circle-green"></div>
            </div>

        </section>

        <!-- FEATURES -->
        <section id="features" class="features-section" aria-labelledby="features-title">

            <h2 id="features-title" class="visually-hidden">
                Features that support your learning
            </h2>

            <div class="feature-grid">

                <article class="feature-card">
                    <span class="feature-icon icon-pink" aria-hidden="true">▤</span>

                    <h3>SPM-ready quizzes</h3>

                    <p>
                        Practise objective and subjective questions across your favourite SPM subjects.
                    </p>
                </article>

                <article class="feature-card">
                    <span class="feature-icon icon-blue" aria-hidden="true">▥</span>

                    <h3>See your progress</h3>

                    <p>
                        Review your scores, explore past attempts and discover which topics need more practice.
                    </p>
                </article>

                <article class="feature-card">
                    <span class="feature-icon icon-green" aria-hidden="true">☆</span>

                    <h3>Learn with feedback</h3>

                    <p>
                        Check objective results and receive teacher feedback on your subjective answers.
                    </p>
                </article>

            </div>
        </section>

        <!-- REGISTRATION BANNER -->
        <section class="signup-banner" aria-labelledby="signup-title">

            <div class="signup-message">
                <span class="feature-icon icon-pink" aria-hidden="true">↗</span>

                <div>
                    <h2 id="signup-title">Ready to level up?</h2>
                    <p>Start building stronger study habits today.</p>
                </div>
            </div>

            <a href="<%= ResolveUrl("~/Register.aspx") %>" class="button button-primary"> Create free account </a>

        </section>

        <!-- HOW IT WORKS -->
        <section id="how-it-works" class="home-section" aria-labelledby="steps-title">

            <div class="section-heading">
                <span class="eyebrow">Your next step</span>
                <h2 id="steps-title">Small steps. Real progress.</h2>
                <p>Getting started with SPMaster is simple.</p>
            </div>

            <div class="steps-grid">

                <article class="step-card">
                    <span class="step-number">01</span>
                    <h3>Create your account</h3>
                    <p>
                        Register as a student to keep your quiz attempts and learning progress in one place.
                    </p>
                </article>

                <article class="step-card">
                    <span class="step-number">02</span>
                    <h3>Pick a subject</h3>
                    <p>
                        Choose a subject and topic, then attempt a quiz at your current learning level.
                    </p>
                </article>

                <article class="step-card">
                    <span class="step-number">03</span>
                    <h3>Review and improve</h3>
                    <p>
                        Review available results and teacher feedback,then practise the topics you find challenging.
                    </p>
                </article>

            </div>

        </section>

        <!-- ABOUT US -->
        <section id="about-us" class="about-section" aria-labelledby="about-title">

            <div class="about-copy">
                <span class="eyebrow">About us</span>

                <h2 id="about-title">
                    A little practice.<br />
                    A lot more confidence.
                </h2>

                <p>
                    SPMaster is a learning platform designed to support Malaysian Form 4 and Form 5 students in their SPM preparation.
                </p>

                <p>
                    We bring organised quizzes, learning resources and meaningful feedback together to help you understand your strengths and work on areas that need improvement.
                </p>
            </div>

            <div class="subjects-panel">
                <h3>Explore our six subjects</h3>

                <ul class="subject-list">
                    <li>Sejarah</li>
                    <li>Bahasa Melayu</li>
                    <li>English</li>
                    <li>Bahasa Cina</li>
                    <li>Mathematics</li>
                    <li>Science</li>
                </ul>

                <p>Made for curious minds and steady progress.</p>
            </div>

        </section>

    </div>

</asp:Content>