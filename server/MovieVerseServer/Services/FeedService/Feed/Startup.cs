using Feed.Data;
using Feed.DTOs.Comment;
using Feed.DTOs.Like;
using Feed.Entities;
using Feed.Repository;
using Feed.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feed
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDatabaseContext, DatabaseContext>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddAutoMapper(configuration =>
            {

                configuration.CreateMap<LikeDTO, Like>().ReverseMap();
                configuration.CreateMap<CommentDTO, Comment>().ReverseMap();

            });

            services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Feed", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Feed v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
