var builder = DistributedApplication.CreateBuilder(args);

//===========================================================================================================================================

var authService = builder.AddProject<Projects.AuthService>("authservice");

var productService = builder.AddProject<Projects.ProductService>("productservice");

var inventoryService = builder.AddProject<Projects.InventoryService>("inventoryservice");

var cartService = builder.AddProject<Projects.CartService>("cartservice");

var orderService = builder.AddProject<Projects.OrderService>("orderservice");

var discountService = builder.AddProject<Projects.DiscountService>("discountservice");

var reviewService = builder.AddProject<Projects.ReviewService>("reviewservice")
                           .WithReference(authService)
                           .WithReference(productService)
                           .WithReference(orderService);

var notificationService = builder.AddProject<Projects.NotificationService>("notificationservice");

var imgBBUploadService = builder.AddProject<Projects.ImgBBUploadService>("imgBBUploadService");

var emailService = builder.AddProject<Projects.EmailService>("emailservice");

//===========================================================================================================================================

//var authService = builder
//    .AddProject<Projects.AuthService>("authservice")
//    .WithHttpEndpoint(port: 10001, name: "http-auth");

//var productService = builder
//    .AddProject<Projects.ProductService>("productservice")
//    .WithHttpEndpoint(port: 10002, name: "http-product");

//var inventoryService = builder
//    .AddProject<Projects.InventoryService>("inventoryservice")
//    .WithHttpEndpoint(port: 10003, name: "http-inventory");

//var cartService = builder
//    .AddProject<Projects.CartService>("cartservice")
//    .WithHttpEndpoint(port: 10004, name: "http-cart");

//var orderService = builder
//    .AddProject<Projects.OrderService>("orderservice")
//    .WithHttpEndpoint(port: 10005, name: "http-order");

//var discountService = builder
//    .AddProject<Projects.DiscountService>("discountservice")
//    .WithHttpEndpoint(port: 10006, name: "http-discount");

//var reviewService = builder
//    .AddProject<Projects.ReviewService>("reviewservice")
//    .WithHttpEndpoint(port: 10007, name: "http-review");

//var notificationService = builder
//    .AddProject<Projects.NotificationService>("notificationservice")
//    .WithHttpEndpoint(port: 10008, name: "http-notification");

//===========================================================================================================================================

authService.WithReference(notificationService);

productService.WithReference(reviewService)
              .WithReference(inventoryService)
              .WithReference(discountService);

inventoryService.WithReference(authService)
                .WithReference(productService)
                .WithReference(notificationService);

cartService.WithReference(productService)
           .WithReference(discountService);

orderService.WithReference(authService)
            .WithReference(productService)
            .WithReference(inventoryService)
            .WithReference(discountService)
            .WithReference(notificationService)
            .WithReference(cartService);

discountService.WithReference(authService)
               .WithReference(notificationService);

//===========================================================================================================================================

builder.Build().Run();
