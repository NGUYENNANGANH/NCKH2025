"use client";
import React from 'react';
import Link from 'next/link';
import Image from 'next/image';

export default function AboutPage() {
  return (
    <>
      {/* Page Title */}
      <div className="page-title light-background">
        <div className="container d-lg-flex justify-content-between align-items-center">
          <h1 className="mb-2 mb-lg-0">About Us</h1>
          <nav className="breadcrumbs">
            <ol>
              <li><Link href="/">Home</Link></li>
              <li className="current">About</li>
            </ol>
          </nav>
        </div>
      </div>
      {/* About Section */}
      <section id="about" className="about section">
        <div className="container">
          <div className="row align-items-center">
            <div className="col-lg-6 mb-4 mb-lg-0">
              <div className="about-image">
                <Image src="/img/about/about-1.webp" alt="About Us" width={600} height={400} className="img-fluid" />
              </div>
            </div>
            <div className="col-lg-6">
              <div className="about-content">
                <h2 className="mb-4">Welcome to eStore</h2>
                <p className="mb-4">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p>
                <p className="mb-4">Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>
                <div className="row g-4">
                  <div className="col-sm-6">
                    <div className="feature-item">
                      <i className="bi bi-truck feature-icon"></i>
                      <h5>Free Shipping</h5>
                      <p>On all orders over $50</p>
                    </div>
                  </div>
                  <div className="col-sm-6">
                    <div className="feature-item">
                      <i className="bi bi-arrow-repeat feature-icon"></i>
                      <h5>Easy Returns</h5>
                      <p>30 days return policy</p>
                    </div>
                  </div>
                  <div className="col-sm-6">
                    <div className="feature-item">
                      <i className="bi bi-shield-check feature-icon"></i>
                      <h5>Secure Payment</h5>
                      <p>100% secure payment</p>
                    </div>
                  </div>
                  <div className="col-sm-6">
                    <div className="feature-item">
                      <i className="bi bi-headset feature-icon"></i>
                      <h5>24/7 Support</h5>
                      <p>Dedicated support</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
      {/* Team Section */}
      <section id="team" className="team section bg-light">
        <div className="container">
          <div className="section-header text-center mb-5">
            <h2>Our Team</h2>
            <p>Meet the people behind eStore</p>
          </div>
          <div className="row g-4">
            <div className="col-lg-3 col-md-6">
              <div className="team-member">
                <div className="team-image">
                  <Image src="/img/team/team-1.webp" alt="Team Member" width={300} height={300} className="img-fluid" />
                </div>
                <div className="team-info text-center">
                  <h5>John Doe</h5>
                  <p className="text-muted">CEO & Founder</p>
                </div>
              </div>
            </div>
            <div className="col-lg-3 col-md-6">
              <div className="team-member">
                <div className="team-image">
                  <Image src="/img/team/team-2.webp" alt="Team Member" width={300} height={300} className="img-fluid" />
                </div>
                <div className="team-info text-center">
                  <h5>Jane Smith</h5>
                  <p className="text-muted">Marketing Director</p>
                </div>
              </div>
            </div>
            <div className="col-lg-3 col-md-6">
              <div className="team-member">
                <div className="team-image">
                  <Image src="/img/team/team-3.webp" alt="Team Member" width={300} height={300} className="img-fluid" />
                </div>
                <div className="team-info text-center">
                  <h5>Mike Johnson</h5>
                  <p className="text-muted">Product Manager</p>
                </div>
              </div>
            </div>
            <div className="col-lg-3 col-md-6">
              <div className="team-member">
                <div className="team-image">
                  <Image src="/img/team/team-4.webp" alt="Team Member" width={300} height={300} className="img-fluid" />
                </div>
                <div className="team-info text-center">
                  <h5>Sarah Williams</h5>
                  <p className="text-muted">Customer Support</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
} 