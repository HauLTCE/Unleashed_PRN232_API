var builder = DistributedApplication.CreateBuilder(args);

//===========================================================================================================================================

var authService = builder.AddProject<Projects.AuthService>("authservice");
var productService = builder.AddProject<Projects.ProductService>("productservice");
var inventoryService = builder.AddProject<Projects.InventoryService>("inventoryservice");
var cartService = builder.AddProject<Projects.CartService>("cartservice");
var orderService = builder.AddProject<Projects.OrderService>("orderservice");
var discountService = builder.AddProject<Projects.DiscountService>("discountservice");
var reviewService = builder.AddProject<Projects.ReviewService>("reviewservice");
var notificationService = builder.AddProject<Projects.NotificationService>("notificationservice");

//===========================================================================================================================================

authService.WithReference(notificationService);

productService.WithReference(reviewService)
              .WithReference(inventoryService)
              .WithReference(discountService);

inventoryService.WithReference(notificationService);

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

reviewService.WithReference(authService)
             .WithReference(productService)
             .WithReference(orderService);





//===========================================================================================================================================

builder.Build().Run();
