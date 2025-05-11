'use client';

import Link from 'next/link';

export default function Footer() {
  return (
    <footer className="footer">
      <div className="container">
        <div className="row">
          <div className="col-md-4 mb-4">
            <h5 className="text-white mb-3">About eStore</h5>
            <p className="text-white-50">
              Your one-stop shop for all your needs. We offer a wide range of products
              with the best quality and competitive prices.
            </p>
            <div className="social-links mt-3">
              <a href="#" className="text-white me-3">
                <i className="bi bi-facebook"></i>
              </a>
              <a href="#" className="text-white me-3">
                <i className="bi bi-twitter"></i>
              </a>
              <a href="#" className="text-white me-3">
                <i className="bi bi-instagram"></i>
              </a>
              <a href="#" className="text-white">
                <i className="bi bi-linkedin"></i>
              </a>
            </div>
          </div>
          <div className="col-md-2 mb-4">
            <h5 className="text-white mb-3">Quick Links</h5>
            <ul className="list-unstyled">
              <li className="mb-2">
                <Link href="/" className="text-white-50 text-decoration-none">
                  Home
                </Link>
              </li>
              <li className="mb-2">
                <Link href="/category" className="text-white-50 text-decoration-none">
                  Shop
                </Link>
              </li>
              <li className="mb-2">
                <Link href="/about" className="text-white-50 text-decoration-none">
                  About
                </Link>
              </li>
              <li className="mb-2">
                <Link href="/contact" className="text-white-50 text-decoration-none">
                  Contact
                </Link>
              </li>
            </ul>
          </div>
          <div className="col-md-3 mb-4">
            <h5 className="text-white mb-3">Customer Service</h5>
            <ul className="list-unstyled">
              <li className="mb-2">
                <Link href="/account" className="text-white-50 text-decoration-none">
                  My Account
                </Link>
              </li>
              <li className="mb-2">
                <Link href="/orders" className="text-white-50 text-decoration-none">
                  Order Tracking
                </Link>
              </li>
              <li className="mb-2">
                <Link href="/wishlist" className="text-white-50 text-decoration-none">
                  Wishlist
                </Link>
              </li>
              <li className="mb-2">
                <Link href="/faq" className="text-white-50 text-decoration-none">
                  FAQ
                </Link>
              </li>
            </ul>
          </div>
          <div className="col-md-3 mb-4">
            <h5 className="text-white mb-3">Contact Info</h5>
            <ul className="list-unstyled text-white-50">
              <li className="mb-2">
                <i className="bi bi-geo-alt me-2"></i>
                123 Street, City, Country
              </li>
              <li className="mb-2">
                <i className="bi bi-telephone me-2"></i>
                +1 (234) 567-890
              </li>
              <li className="mb-2">
                <i className="bi bi-envelope me-2"></i>
                info@estore.com
              </li>
            </ul>
          </div>
        </div>
        <hr className="border-secondary" />
        <div className="row">
          <div className="col-md-6">
            <p className="text-white-50 mb-0">
              &copy; {new Date().getFullYear()} eStore. All rights reserved.
            </p>
          </div>
          <div className="col-md-6 text-md-end">
            <img
              src="/images/payment-methods.png"
              alt="Payment Methods"
              className="img-fluid"
              style={{ maxHeight: '30px' }}
            />
          </div>
        </div>
      </div>
    </footer>
  );
} 