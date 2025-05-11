"use client";
import React, { useState } from "react";
import Link from "next/link";
import Image from "next/image";

const TABS = ["profile", "orders", "wishlist", "settings"];

export default function AccountPage() {
  const [activeTab, setActiveTab] = useState("profile");

  return (
    <>
      {/* Page Title */}
      <div className="page-title light-background">
        <div className="container d-lg-flex justify-content-between align-items-center">
          <h1 className="mb-2 mb-lg-0">My Account</h1>
          <nav className="breadcrumbs">
            <ol>
              <li><Link href="/">Home</Link></li>
              <li className="current">Account</li>
            </ol>
          </nav>
        </div>
      </div>
      {/* Account Section */}
      <section className="account section">
        <div className="container">
          <div className="row">
            {/* Sidebar */}
            <aside className="col-lg-3 mb-4 mb-lg-0">
              <div className="account-sidebar card">
                <div className="card-body p-4">
                  <div className="account-avatar text-center mb-4">
                    <Image src="/img/person/person-m-1.webp" alt="Avatar" width={100} height={100} className="rounded-circle" />
                    <h5 className="mt-3 mb-0">John Doe</h5>
                    <span className="text-muted">johndoe@email.com</span>
                  </div>
                  <ul className="nav flex-column nav-pills">
                    <li className="nav-item">
                      <button className={`nav-link${activeTab === "profile" ? " active" : ""}`} onClick={() => setActiveTab("profile")}>Profile</button>
                    </li>
                    <li className="nav-item">
                      <button className={`nav-link${activeTab === "orders" ? " active" : ""}`} onClick={() => setActiveTab("orders")}>Orders</button>
                    </li>
                    <li className="nav-item">
                      <button className={`nav-link${activeTab === "wishlist" ? " active" : ""}`} onClick={() => setActiveTab("wishlist")}>Wishlist</button>
                    </li>
                    <li className="nav-item">
                      <button className={`nav-link${activeTab === "settings" ? " active" : ""}`} onClick={() => setActiveTab("settings")}>Settings</button>
                    </li>
                    <li className="nav-item">
                      <Link className="nav-link text-danger" href="/login-register">Logout</Link>
                    </li>
                  </ul>
                </div>
              </div>
            </aside>
            {/* Main Content */}
            <div className="col-lg-9">
              <div className="tab-content">
                {/* Profile Tab */}
                {activeTab === "profile" && (
                  <div className="tab-pane fade show active" id="profile">
                    <div className="card mb-4">
                      <div className="card-body p-4">
                        <h4 className="mb-4">Profile Information</h4>
                        <form>
                          <div className="row g-3">
                            <div className="col-md-6">
                              <label className="form-label">First Name</label>
                              <input type="text" className="form-control" value="John" readOnly />
                            </div>
                            <div className="col-md-6">
                              <label className="form-label">Last Name</label>
                              <input type="text" className="form-control" value="Doe" readOnly />
                            </div>
                            <div className="col-12">
                              <label className="form-label">Email</label>
                              <input type="email" className="form-control" value="johndoe@email.com" readOnly />
                            </div>
                            <div className="col-12">
                              <label className="form-label">Phone</label>
                              <input type="text" className="form-control" value="+1 (234) 567-890" readOnly />
                            </div>
                          </div>
                        </form>
                      </div>
                    </div>
                  </div>
                )}
                {/* Orders Tab */}
                {activeTab === "orders" && (
                  <div className="tab-pane fade show active" id="orders">
                    <div className="card mb-4">
                      <div className="card-body p-4">
                        <h4 className="mb-4">My Orders</h4>
                        <div className="table-responsive">
                          <table className="table">
                            <thead>
                              <tr>
                                <th>Order #</th>
                                <th>Date</th>
                                <th>Status</th>
                                <th>Total</th>
                                <th>Action</th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr>
                                <td>1001</td>
                                <td>2024-05-01</td>
                                <td><span className="badge bg-success">Delivered</span></td>
                                <td>$249.99</td>
                                <td><button className="btn btn-sm btn-outline-primary">View</button></td>
                              </tr>
                              <tr>
                                <td>1002</td>
                                <td>2024-04-15</td>
                                <td><span className="badge bg-warning">Pending</span></td>
                                <td>$129.99</td>
                                <td><button className="btn btn-sm btn-outline-primary">View</button></td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
                {/* Wishlist Tab */}
                {activeTab === "wishlist" && (
                  <div className="tab-pane fade show active" id="wishlist">
                    <div className="card mb-4">
                      <div className="card-body p-4">
                        <h4 className="mb-4">My Wishlist</h4>
                        <div className="row g-3">
                          <div className="col-md-6 col-lg-4">
                            <div className="product-card">
                              <div className="product-image">
                                <Image src="/img/product/product-1.webp" alt="Product" width={200} height={200} className="img-fluid" />
                              </div>
                              <div className="product-info">
                                <h5>Premium Headphones</h5>
                                <p className="price">$129.99</p>
                                <button className="btn btn-sm btn-primary">Add to Cart</button>
                              </div>
                            </div>
                          </div>
                          <div className="col-md-6 col-lg-4">
                            <div className="product-card">
                              <div className="product-image">
                                <Image src="/img/product/product-2.webp" alt="Product" width={200} height={200} className="img-fluid" />
                              </div>
                              <div className="product-info">
                                <h5>Smart Watch</h5>
                                <p className="price">$199.99</p>
                                <button className="btn btn-sm btn-primary">Add to Cart</button>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
                {/* Settings Tab */}
                {activeTab === "settings" && (
                  <div className="tab-pane fade show active" id="settings">
                    <div className="card mb-4">
                      <div className="card-body p-4">
                        <h4 className="mb-4">Account Settings</h4>
                        <form>
                          <div className="row g-3">
                            <div className="col-12">
                              <label className="form-label">Change Password</label>
                              <input type="password" className="form-control" placeholder="New Password" />
                            </div>
                            <div className="col-12">
                              <label className="form-label">Confirm Password</label>
                              <input type="password" className="form-control" placeholder="Confirm Password" />
                            </div>
                            <div className="col-12 text-end">
                              <button type="submit" className="btn btn-primary">Update Password</button>
                            </div>
                          </div>
                        </form>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
} 