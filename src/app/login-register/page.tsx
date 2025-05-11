'use client';

import React, { useState } from 'react';
import Link from 'next/link';
import Image from 'next/image';

export default function LoginRegisterPage() {
  const [activeTab, setActiveTab] = useState<'login' | 'register'>('login');
  const [loginData, setLoginData] = useState({
    email: '',
    password: '',
    rememberMe: false
  });
  const [registerData, setRegisterData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
    agreeTerms: false
  });

  const handleLoginChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setLoginData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleRegisterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setRegisterData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleLoginSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Handle login submission
    console.log('Login submitted:', loginData);
  };

  const handleRegisterSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Handle register submission
    console.log('Register submitted:', registerData);
  };

  return (
    <div className="login-register-page">
      <header id="header" className="header position-relative">
        {/* Top Bar */}
        <div className="top-bar py-2">
          <div className="container-fluid container-xl">
            <div className="row align-items-center">
              <div className="col-lg-4 d-none d-lg-flex">
                <div className="top-bar-item">
                  <i className="bi bi-telephone-fill me-2"></i>
                  <span>Need help? Call us: </span>
                  <a href="tel:+1234567890">+1 (234) 567-890</a>
                </div>
              </div>

              <div className="col-lg-4 col-md-12 text-center">
                <div className="announcement-slider swiper init-swiper">
                  <script type="application/json" className="swiper-config">
                    {JSON.stringify({
                      loop: true,
                      speed: 600,
                      autoplay: {
                        delay: 5000
                      },
                      slidesPerView: 1,
                      direction: "vertical",
                      effect: "slide"
                    })}
                  </script>
                  <div className="swiper-wrapper">
                    <div className="swiper-slide">🚚 Free shipping on orders over $50</div>
                    <div className="swiper-slide">💰 30 days money back guarantee.</div>
                    <div className="swiper-slide">🎁 20% off on your first order</div>
                  </div>
                </div>
              </div>

              <div className="col-lg-4 d-none d-lg-block">
                <div className="d-flex justify-content-end">
                  <div className="top-bar-item dropdown me-3">
                    <a href="#" className="dropdown-toggle" data-bs-toggle="dropdown">
                      <i className="bi bi-translate me-2"></i>EN
                    </a>
                    <ul className="dropdown-menu">
                      <li><a className="dropdown-item" href="#"><i className="bi bi-check2 me-2 selected-icon"></i>English</a></li>
                      <li><a className="dropdown-item" href="#">Español</a></li>
                      <li><a className="dropdown-item" href="#">Français</a></li>
                      <li><a className="dropdown-item" href="#">Deutsch</a></li>
                    </ul>
                  </div>
                  <div className="top-bar-item dropdown">
                    <a href="#" className="dropdown-toggle" data-bs-toggle="dropdown">
                      <i className="bi bi-currency-dollar me-2"></i>USD
                    </a>
                    <ul className="dropdown-menu">
                      <li><a className="dropdown-item" href="#"><i className="bi bi-check2 me-2 selected-icon"></i>USD</a></li>
                      <li><a className="dropdown-item" href="#">EUR</a></li>
                      <li><a className="dropdown-item" href="#">GBP</a></li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Main Header */}
        <div className="main-header">
          <div className="container-fluid container-xl">
            <div className="d-flex py-3 align-items-center justify-content-between">
              {/* Logo */}
              <Link href="/" className="logo d-flex align-items-center">
                <h1 className="sitename">eStore</h1>
              </Link>

              {/* Search */}
              <form className="search-form desktop-search-form">
                <div className="input-group">
                  <input type="text" className="form-control" placeholder="Search for products" />
                  <button className="btn" type="submit">
                    <i className="bi bi-search"></i>
                  </button>
                </div>
              </form>

              {/* Actions */}
              <div className="header-actions d-flex align-items-center justify-content-end">
                {/* Mobile Search Toggle */}
                <button className="header-action-btn mobile-search-toggle d-xl-none" type="button" data-bs-toggle="collapse" data-bs-target="#mobileSearch" aria-expanded="false" aria-controls="mobileSearch">
                  <i className="bi bi-search"></i>
                </button>

                {/* Account */}
                <div className="dropdown account-dropdown">
                  <button className="header-action-btn" data-bs-toggle="dropdown">
                    <i className="bi bi-person"></i>
                  </button>
                  <div className="dropdown-menu">
                    <div className="dropdown-header">
                      <h6>Welcome to <span className="sitename">eStore</span></h6>
                      <p className="mb-0">Access account &amp; manage orders</p>
                    </div>
                    <div className="dropdown-body">
                      <Link className="dropdown-item d-flex align-items-center" href="/account">
                        <i className="bi bi-person-circle me-2"></i>
                        <span>My Profile</span>
                      </Link>
                      <Link className="dropdown-item d-flex align-items-center" href="/account">
                        <i className="bi bi-bag-check me-2"></i>
                        <span>My Orders</span>
                      </Link>
                      <Link className="dropdown-item d-flex align-items-center" href="/account">
                        <i className="bi bi-heart me-2"></i>
                        <span>My Wishlist</span>
                      </Link>
                      <Link className="dropdown-item d-flex align-items-center" href="/account">
                        <i className="bi bi-gear me-2"></i>
                        <span>Settings</span>
                      </Link>
                    </div>
                    <div className="dropdown-footer">
                      <Link href="/login-register" className="btn btn-primary w-100 mb-2">Sign In</Link>
                      <Link href="/login-register" className="btn btn-outline-primary w-100">Register</Link>
                    </div>
                  </div>
                </div>

                {/* Wishlist */}
                <Link href="/account" className="header-action-btn d-none d-md-block">
                  <i className="bi bi-heart"></i>
                  <span className="badge">0</span>
                </Link>

                {/* Cart */}
                <Link href="/cart" className="header-action-btn">
                  <i className="bi bi-cart3"></i>
                  <span className="badge">3</span>
                </Link>

                {/* Mobile Navigation Toggle */}
                <i className="mobile-nav-toggle d-xl-none bi bi-list me-0"></i>
              </div>
            </div>
          </div>
        </div>

        {/* Navigation */}
        <div className="header-nav">
          <div className="container-fluid container-xl">
            <div className="position-relative">
              <nav id="navmenu" className="navmenu">
                <ul>
                  <li><Link href="/">Home</Link></li>
                  <li><Link href="/about">About</Link></li>
                  <li><Link href="/category">Category</Link></li>
                  <li><Link href="/product-details">Product Details</Link></li>
                  <li><Link href="/cart">Cart</Link></li>
                  <li><Link href="/checkout">Checkout</Link></li>
                  <li className="dropdown">
                    <a href="#"><span>Dropdown</span> <i className="bi bi-chevron-down toggle-dropdown"></i></a>
                    <ul>
                      <li><a href="#">Dropdown 1</a></li>
                      <li className="dropdown">
                        <a href="#"><span>Deep Dropdown</span> <i className="bi bi-chevron-down toggle-dropdown"></i></a>
                        <ul>
                          <li><a href="#">Deep Dropdown 1</a></li>
                          <li><a href="#">Deep Dropdown 2</a></li>
                          <li><a href="#">Deep Dropdown 3</a></li>
                          <li><a href="#">Deep Dropdown 4</a></li>
                          <li><a href="#">Deep Dropdown 5</a></li>
                        </ul>
                      </li>
                      <li><a href="#">Dropdown 2</a></li>
                      <li><a href="#">Dropdown 3</a></li>
                      <li><a href="#">Dropdown 4</a></li>
                    </ul>
                  </li>

                  {/* Products Mega Menu 1 */}
                  <li className="products-megamenu-1">
                    <a href="#"><span>Megamenu 1</span> <i className="bi bi-chevron-down toggle-dropdown"></i></a>
                    <ul className="mobile-megamenu">
                      <li><a href="#">Featured Products</a></li>
                      <li><a href="#">New Arrivals</a></li>
                      <li><a href="#">Sale Items</a></li>
                      <li className="dropdown">
                        <a href="#"><span>Clothing</span> <i className="bi bi-chevron-down toggle-dropdown"></i></a>
                        <ul>
                          <li><a href="#">Men's Wear</a></li>
                          <li><a href="#">Women's Wear</a></li>
                          <li><a href="#">Kids Collection</a></li>
                          <li><a href="#">Sportswear</a></li>
                          <li><a href="#">Accessories</a></li>
                        </ul>
                      </li>
                      <li className="dropdown">
                        <a href="#"><span>Electronics</span> <i className="bi bi-chevron-down toggle-dropdown"></i></a>
                        <ul>
                          <li><a href="#">Smartphones</a></li>
                          <li><a href="#">Laptops</a></li>
                          <li><a href="#">Audio Devices</a></li>
                          <li><a href="#">Smart Home</a></li>
                          <li><a href="#">Accessories</a></li>
                        </ul>
                      </li>
                    </ul>
                  </li>
                </ul>
              </nav>
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main id="main">
        {/* Login Register Section */}
        <section className="login-register-section py-5">
          <div className="container">
            <div className="row justify-content-center">
              <div className="col-lg-6">
                <div className="card">
                  <div className="card-body p-4">
                    {/* Tabs */}
                    <ul className="nav nav-tabs mb-4" role="tablist">
                      <li className="nav-item" role="presentation">
                        <button
                          className={`nav-link ${activeTab === 'login' ? 'active' : ''}`}
                          onClick={() => setActiveTab('login')}
                          type="button"
                          role="tab"
                        >
                          Login
                        </button>
                      </li>
                      <li className="nav-item" role="presentation">
                        <button
                          className={`nav-link ${activeTab === 'register' ? 'active' : ''}`}
                          onClick={() => setActiveTab('register')}
                          type="button"
                          role="tab"
                        >
                          Register
                        </button>
                      </li>
                    </ul>

                    {/* Tab Content */}
                    <div className="tab-content">
                      {/* Login Form */}
                      <div className={`tab-pane fade ${activeTab === 'login' ? 'show active' : ''}`}>
                        <form onSubmit={handleLoginSubmit}>
                          <div className="mb-3">
                            <label htmlFor="loginEmail" className="form-label">Email</label>
                            <input
                              type="email"
                              className="form-control"
                              id="loginEmail"
                              name="email"
                              value={loginData.email}
                              onChange={handleLoginChange}
                              required
                            />
                          </div>
                          <div className="mb-3">
                            <label htmlFor="loginPassword" className="form-label">Password</label>
                            <input
                              type="password"
                              className="form-control"
                              id="loginPassword"
                              name="password"
                              value={loginData.password}
                              onChange={handleLoginChange}
                              required
                            />
                          </div>
                          <div className="mb-3 form-check">
                            <input
                              type="checkbox"
                              className="form-check-input"
                              id="rememberMe"
                              name="rememberMe"
                              checked={loginData.rememberMe}
                              onChange={handleLoginChange}
                            />
                            <label className="form-check-label" htmlFor="rememberMe">
                              Remember me
                            </label>
                          </div>
                          <button type="submit" className="btn btn-primary w-100">
                            Login
                          </button>
                        </form>
                      </div>

                      {/* Register Form */}
                      <div className={`tab-pane fade ${activeTab === 'register' ? 'show active' : ''}`}>
                        <form onSubmit={handleRegisterSubmit}>
                          <div className="row">
                            <div className="col-md-6 mb-3">
                              <label htmlFor="firstName" className="form-label">First Name</label>
                              <input
                                type="text"
                                className="form-control"
                                id="firstName"
                                name="firstName"
                                value={registerData.firstName}
                                onChange={handleRegisterChange}
                                required
                              />
                            </div>
                            <div className="col-md-6 mb-3">
                              <label htmlFor="lastName" className="form-label">Last Name</label>
                              <input
                                type="text"
                                className="form-control"
                                id="lastName"
                                name="lastName"
                                value={registerData.lastName}
                                onChange={handleRegisterChange}
                                required
                              />
                            </div>
                          </div>
                          <div className="mb-3">
                            <label htmlFor="registerEmail" className="form-label">Email</label>
                            <input
                              type="email"
                              className="form-control"
                              id="registerEmail"
                              name="email"
                              value={registerData.email}
                              onChange={handleRegisterChange}
                              required
                            />
                          </div>
                          <div className="mb-3">
                            <label htmlFor="registerPassword" className="form-label">Password</label>
                            <input
                              type="password"
                              className="form-control"
                              id="registerPassword"
                              name="password"
                              value={registerData.password}
                              onChange={handleRegisterChange}
                              required
                            />
                          </div>
                          <div className="mb-3">
                            <label htmlFor="confirmPassword" className="form-label">Confirm Password</label>
                            <input
                              type="password"
                              className="form-control"
                              id="confirmPassword"
                              name="confirmPassword"
                              value={registerData.confirmPassword}
                              onChange={handleRegisterChange}
                              required
                            />
                          </div>
                          <div className="mb-3 form-check">
                            <input
                              type="checkbox"
                              className="form-check-input"
                              id="agreeTerms"
                              name="agreeTerms"
                              checked={registerData.agreeTerms}
                              onChange={handleRegisterChange}
                              required
                            />
                            <label className="form-check-label" htmlFor="agreeTerms">
                              I agree to the <a href="#">Terms & Conditions</a>
                            </label>
                          </div>
                          <button type="submit" className="btn btn-primary w-100">
                            Register
                          </button>
                        </form>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>
      </main>

      {/* Footer */}
      <footer id="footer" className="footer">
        <div className="container">
          <div className="copyright">
            &copy; Copyright <strong><span>eStore</span></strong>. All Rights Reserved
          </div>
        </div>
      </footer>
    </div>
  );
} 