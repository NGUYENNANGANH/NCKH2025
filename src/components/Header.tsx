'use client';

import Link from 'next/link';
import { useState, useEffect } from 'react';

export default function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isScrolled, setIsScrolled] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 50);
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
    if (!isMenuOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'unset';
    }
  };

  return (
    <header className={`header ${isScrolled ? 'scrolled' : ''}`}>
      {/* Top Bar */}
      <div className="top-bar py-2">
        <div className="container">
          <div className="row align-items-center">
            <div className="col-md-6">
              <div className="d-flex gap-3">
                <a href="tel:+1234567890" className="text-decoration-none text-dark">
                  <i className="bi bi-telephone me-2"></i>+1 (234) 567-890
                </a>
                <a href="mailto:info@estore.com" className="text-decoration-none text-dark">
                  <i className="bi bi-envelope me-2"></i>info@estore.com
                </a>
              </div>
            </div>
            <div className="col-md-6 text-end">
              <div className="d-flex justify-content-end gap-3">
                <Link href="/account" className="text-decoration-none text-dark">
                  <i className="bi bi-person me-2"></i>My Account
                </Link>
                <Link href="/wishlist" className="text-decoration-none text-dark">
                  <i className="bi bi-heart me-2"></i>Wishlist
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Main Header */}
      <div className="main-header py-3">
        <div className="container">
          <div className="row align-items-center">
            <div className="col-md-3">
              <Link href="/" className="text-decoration-none">
                <h1 className="h3 mb-0">eStore</h1>
              </Link>
            </div>
            <div className="col-md-6">
              <form className="d-flex">
                <input
                  type="search"
                  className="form-control"
                  placeholder="Search products..."
                />
                <button className="btn btn-primary ms-2">
                  <i className="bi bi-search"></i>
                </button>
              </form>
            </div>
            <div className="col-md-3">
              <div className="header-actions d-flex justify-content-end gap-3">
                <Link href="/cart" className="header-action-btn">
                  <i className="bi bi-cart3 fs-5"></i>
                  <span className="badge bg-primary rounded-pill">3</span>
                </Link>
                <button
                  className="header-action-btn d-md-none"
                  onClick={toggleMenu}
                  aria-label="Toggle menu"
                >
                  <i className={`bi bi-${isMenuOpen ? 'x' : 'list'} fs-5`}></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="header-nav">
        <div className="container">
          <div className={`navmenu ${isMenuOpen ? 'active' : ''}`}>
            <ul className="d-flex flex-md-row flex-column">
              <li>
                <Link href="/">Home</Link>
              </li>
              <li>
                <Link href="/about">About</Link>
              </li>
              <li>
                <Link href="/category">Category</Link>
              </li>
              <li>
                <Link href="/product-details">Product Details</Link>
              </li>
              <li>
                <Link href="/cart">Cart</Link>
              </li>
              <li>
                <Link href="/checkout">Checkout</Link>
              </li>
              <li className="dropdown">
                <a href="#" className="dropdown-toggle">Dropdown</a>
                <ul className="dropdown-menu">
                  <li><a href="#">Dropdown 1</a></li>
                  <li><a href="#">Dropdown 2</a></li>
                  <li><a href="#">Dropdown 3</a></li>
                </ul>
              </li>
              <li className="dropdown">
                <a href="#" className="dropdown-toggle">Megamenu 1</a>
                <ul className="dropdown-menu">
                  <li><a href="#">Featured Products</a></li>
                  <li><a href="#">New Arrivals</a></li>
                  <li><a href="#">Sale Items</a></li>
                </ul>
              </li>
              <li className="dropdown">
                <a href="#" className="dropdown-toggle">Megamenu 2</a>
                <ul className="dropdown-menu">
                  <li><a href="#">Clothing</a></li>
                  <li><a href="#">Electronics</a></li>
                </ul>
              </li>
              <li>
                <Link href="/contact">Contact</Link>
              </li>
            </ul>
          </div>
        </div>
      </nav>

      {/* Mobile Menu Overlay */}
      {isMenuOpen && (
        <div className="mobile-menu-overlay" onClick={toggleMenu}></div>
      )}
    </header>
  );
} 