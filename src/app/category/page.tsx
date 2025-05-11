"use client";
import React from "react";
import Link from "next/link";
import Image from "next/image";

export default function CategoryPage() {
  return (
    <>
      {/* Page Title */}
      <div className="page-title light-background">
        <div className="container d-lg-flex justify-content-between align-items-center">
          <h1 className="mb-2 mb-lg-0">Category</h1>
          <nav className="breadcrumbs">
            <ol>
              <li><Link href="/">Home</Link></li>
              <li className="current">Category</li>
            </ol>
          </nav>
        </div>
      </div>
      {/* Category Section */}
      <section className="category section">
        <div className="container">
          <div className="row">
            {/* Sidebar */}
            <aside className="col-lg-3 mb-4 mb-lg-0">
              <div className="sidebar">
                {/* Categories */}
                <div className="sidebar-widget mb-4">
                  <h4 className="widget-title">Categories</h4>
                  <ul className="list-unstyled">
                    <li><a href="#" className="active">All Products</a></li>
                    <li><a href="#">Electronics</a></li>
                    <li><a href="#">Clothing</a></li>
                    <li><a href="#">Home & Kitchen</a></li>
                    <li><a href="#">Beauty & Personal Care</a></li>
                    <li><a href="#">Sports & Outdoors</a></li>
                  </ul>
                </div>
                {/* Price Range */}
                <div className="sidebar-widget mb-4">
                  <h4 className="widget-title">Price Range</h4>
                  <div className="price-range">
                    <input type="range" className="form-range" min="0" max="1000" step="10" />
                    <div className="d-flex justify-content-between mt-2">
                      <span>$0</span>
                      <span>$1000</span>
                    </div>
                  </div>
                </div>
                {/* Brands */}
                <div className="sidebar-widget mb-4">
                  <h4 className="widget-title">Brands</h4>
                  <div className="form-check">
                    <input className="form-check-input" type="checkbox" id="brand1" />
                    <label className="form-check-label" htmlFor="brand1">Apple</label>
                  </div>
                  <div className="form-check">
                    <input className="form-check-input" type="checkbox" id="brand2" />
                    <label className="form-check-label" htmlFor="brand2">Samsung</label>
                  </div>
                  <div className="form-check">
                    <input className="form-check-input" type="checkbox" id="brand3" />
                    <label className="form-check-label" htmlFor="brand3">Sony</label>
                  </div>
                  <div className="form-check">
                    <input className="form-check-input" type="checkbox" id="brand4" />
                    <label className="form-check-label" htmlFor="brand4">LG</label>
                  </div>
                </div>
                {/* Rating */}
                <div className="sidebar-widget">
                  <h4 className="widget-title">Rating</h4>
                  <div className="form-check">
                    <input className="form-check-input" type="checkbox" id="rating5" />
                    <label className="form-check-label" htmlFor="rating5">
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                    </label>
                  </div>
                  <div className="form-check">
                    <input className="form-check-input" type="checkbox" id="rating4" />
                    <label className="form-check-label" htmlFor="rating4">
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star text-warning"></i>
                    </label>
                  </div>
                  <div className="form-check">
                    <input className="form-check-input" type="checkbox" id="rating3" />
                    <label className="form-check-label" htmlFor="rating3">
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star-fill text-warning"></i>
                      <i className="bi bi-star text-warning"></i>
                      <i className="bi bi-star text-warning"></i>
                    </label>
                  </div>
                </div>
              </div>
            </aside>
            {/* Products Grid */}
            <div className="col-lg-9">
              {/* Filters */}
              <div className="filters mb-4">
                <div className="row align-items-center">
                  <div className="col-md-6 mb-3 mb-md-0">
                    <div className="d-flex align-items-center">
                      <span className="me-2">Sort by:</span>
                      <select className="form-select">
                        <option>Featured</option>
                        <option>Price: Low to High</option>
                        <option>Price: High to Low</option>
                        <option>Newest</option>
                        <option>Best Selling</option>
                      </select>
                    </div>
                  </div>
                  <div className="col-md-6">
                    <div className="d-flex justify-content-md-end">
                      <div className="view-options">
                        <button className="btn btn-outline-secondary me-2"><i className="bi bi-grid"></i></button>
                        <button className="btn btn-outline-secondary"><i className="bi bi-list"></i></button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              {/* Products List (replace with your product grid) */}
              <div className="row g-4">
                {/* Example Product Card */}
                <div className="col-md-4">
                  <div className="product-card">
                    <div className="product-image">
                      <Image src="/images/products/headphones.jpg" alt="Product" width={300} height={300} className="img-fluid" />
                    </div>
                    <div className="product-info">
                      <h5>Wireless Headphones</h5>
                      <p className="price">$99.99</p>
                      <button className="btn btn-primary w-100">Add to Cart</button>
                    </div>
                  </div>
                </div>
                {/* ...repeat for more products... */}
              </div>
              {/* Pagination */}
              <nav className="mt-5">
                <ul className="pagination justify-content-center">
                  <li className="page-item disabled">
                    <a className="page-link" href="#" tabIndex={-1} aria-disabled="true">Previous</a>
                  </li>
                  <li className="page-item active"><a className="page-link" href="#">1</a></li>
                  <li className="page-item"><a className="page-link" href="#">2</a></li>
                  <li className="page-item"><a className="page-link" href="#">3</a></li>
                  <li className="page-item">
                    <a className="page-link" href="#">Next</a>
                  </li>
                </ul>
              </nav>
            </div>
          </div>
        </div>
      </section>
    </>
  );
} 