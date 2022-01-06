using IdentityandJwt.Api2.CustomValidation;
using IdentityandJwt.Api2.Model;
using IdentityandJwt.Api2.Model.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2
{
    public class Startup
    {
        String CorsPolicy = "CorsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MvcMovieContext")));
           
            services.AddIdentity<AppUser, IdentityRole>(opts=>{

                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 3;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;

            }).AddPasswordValidator<CustomPasswordValidator>().AddEntityFrameworkStores<AppIdentityDbContext>();
            //CookieBuilder cookieBuilder = new CookieBuilder();
            //cookieBuilder.Name = "MyBlog";
            //cookieBuilder.HttpOnly = false;
            //cookieBuilder.Expiration = System.TimeSpan.FromHours(3);
            //cookieBuilder.SameSite = SameSiteMode.Strict;

            //services.ConfigureApplicationCookie(opts =>
            //{
            //    opts.LoginPath = new PathString("/api/Account/Login");
            //    opts.Cookie = cookieBuilder;
            //    opts.SlidingExpiration = true;
            //}) ;
            //           services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials() );
            //});

            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsPolicy,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:3000")
                                      //builder.WithOrigins("http://10.10.10.190:5000",
                                      //                  "http://10.10.10.80:5000",
                                      //                    "http://localhost:60999",
                                      //                    "http://10.10.10.192:3000"
                                      //                    //, "null"
                                      //                    ) // null must remove on Product
                                      //                      // .AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials()
                                      ;
                                  });
            });
            services.AddControllers();
            //services.AddCors(opt =>
            //{
            //    opt.AddDefaultPolicy(builder =>
            //    {
            //        builder.AllowAnyOrigin()
            //            .AllowAnyHeader()
            //            .AllowAnyMethod();
            //    });
            //});
            //services.AddCors(c =>
            //{
            //    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            //});
            //services.AddCors(options =>
            //     options.AddDefaultPolicy(builder =>
            //     builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My Web Api",
                    Version = "V1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Jwt Authorization0",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                }

                    );
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Token:Issuer"],
                    ValidAudience = Configuration["Token:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:SecurityKey"])),
                    ClockSkew = TimeSpan.Zero
                   // ClockSkew = TimeSpan.Zero
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
         
            app.UseRouting();
            app.UseCors(CorsPolicy);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");

            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
