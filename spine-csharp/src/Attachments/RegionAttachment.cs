/******************************************************************************
 * Spine Runtime Software License - Version 1.0
 * 
 * Copyright (c) 2013, Esoteric Software
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms in whole or in part, with
 * or without modification, are permitted provided that the following conditions
 * are met:
 * 
 * 1. A Spine Single User License or Spine Professional License must be
 *    purchased from Esoteric Software and the license must remain valid:
 *    http://esotericsoftware.com/
 * 2. Redistributions of source code must retain this license, which is the
 *    above copyright notice, this declaration of conditions and the following
 *    disclaimer.
 * 3. Redistributions in binary form must reproduce this license, which is the
 *    above copyright notice, this declaration of conditions and the following
 *    disclaimer, in the documentation and/or other materials provided with the
 *    distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;

namespace Spine {
	/// <summary>Attachment that displays a texture region.</summary>
	public class RegionAttachment : Attachment {
		public const int X1 = 0;
		public const int Y1 = 1;
		public const int X2 = 2;
		public const int Y2 = 3;
		public const int X3 = 4;
		public const int Y3 = 5;
		public const int X4 = 6;
		public const int Y4 = 7;

		internal float x, y, rotation, scaleX = 1, scaleY = 1, width, height;
		internal float regionOffsetX, regionOffsetY, regionWidth, regionHeight, regionOriginalWidth, regionOriginalHeight;
		internal float[] offset = new float[8], uvs = new float[8];

		public float X { get { return x; } set { x = value; } }
		public float Y { get { return y; } set { y = value; } }
		public float Rotation { get { return rotation; } set { rotation = value; } }
		public float ScaleX { get { return scaleX; } set { scaleX = value; } }
		public float ScaleY { get { return scaleY; } set { scaleY = value; } }
		public float Width { get { return width; } set { width = value; } }
		public float Height { get { return height; } set { height = value; } }

		public Object RendererObject { get; set; }
		public float RegionOffsetX { get { return regionOffsetX; } set { regionOffsetX = value; } }
		public float RegionOffsetY { get { return regionOffsetY; } set { regionOffsetY = value; } } // Pixels stripped from the bottom left, unrotated.
		public float RegionWidth { get { return regionWidth; } set { regionWidth = value; } }
		public float RegionHeight { get { return regionHeight; } set { regionHeight = value; } } // Unrotated, stripped size.
		public float RegionOriginalWidth { get { return regionOriginalWidth; } set { regionOriginalWidth = value; } }
		public float RegionOriginalHeight { get { return regionOriginalHeight; } set { regionOriginalHeight = value; } } // Unrotated, unstripped size.

		public float[] Offset { get { return offset; } }
		public float[] UVs { get { return uvs; } }

		public RegionAttachment (string name)
			: base(name) {
		}

		public void SetUVs (float u, float v, float u2, float v2, bool rotate) {
			float[] uvs = this.uvs;
			if (rotate) {
				uvs[X2] = u;
				uvs[Y2] = v2;
				uvs[X3] = u;
				uvs[Y3] = v;
				uvs[X4] = u2;
				uvs[Y4] = v;
				uvs[X1] = u2;
				uvs[Y1] = v2;
			} else {
				uvs[X1] = u;
				uvs[Y1] = v2;
				uvs[X2] = u;
				uvs[Y2] = v;
				uvs[X3] = u2;
				uvs[Y3] = v;
				uvs[X4] = u2;
				uvs[Y4] = v2;
			}
		}

		public void UpdateOffset () {
			float width = this.width;
			float height = this.height;
			float scaleX = this.scaleX;
			float scaleY = this.scaleY;
			float regionScaleX = width / regionOriginalWidth * scaleX;
			float regionScaleY = height / regionOriginalHeight * scaleY;
			float localX = -width / 2 * scaleX + regionOffsetX * regionScaleX;
			float localY = -height / 2 * scaleY + regionOffsetY * regionScaleY;
			float localX2 = localX + regionWidth * regionScaleX;
			float localY2 = localY + regionHeight * regionScaleY;
			float radians = rotation * (float)Math.PI / 180;
			float cos = (float)Math.Cos(radians);
			float sin = (float)Math.Sin(radians);
			float x = this.x;
			float y = this.y;
			float localXCos = localX * cos + x;
			float localXSin = localX * sin;
			float localYCos = localY * cos + y;
			float localYSin = localY * sin;
			float localX2Cos = localX2 * cos + x;
			float localX2Sin = localX2 * sin;
			float localY2Cos = localY2 * cos + y;
			float localY2Sin = localY2 * sin;
			float[] offset = this.offset;
			offset[X1] = localXCos - localYSin;
			offset[Y1] = localYCos + localXSin;
			offset[X2] = localXCos - localY2Sin;
			offset[Y2] = localY2Cos + localXSin;
			offset[X3] = localX2Cos - localY2Sin;
			offset[Y3] = localY2Cos + localX2Sin;
			offset[X4] = localX2Cos - localYSin;
			offset[Y4] = localYCos + localX2Sin;
		}

		public void ComputeWorldVertices (float x, float y, Bone bone, float[] vertices) {
			x += bone.worldX;
			y += bone.worldY;
			float m00 = bone.m00;
			float m01 = bone.m01;
			float m10 = bone.m10;
			float m11 = bone.m11;
			float[] offset = this.offset;
			vertices[X1] = offset[X1] * m00 + offset[Y1] * m01 + x;
			vertices[Y1] = offset[X1] * m10 + offset[Y1] * m11 + y;
			vertices[X2] = offset[X2] * m00 + offset[Y2] * m01 + x;
			vertices[Y2] = offset[X2] * m10 + offset[Y2] * m11 + y;
			vertices[X3] = offset[X3] * m00 + offset[Y3] * m01 + x;
			vertices[Y3] = offset[X3] * m10 + offset[Y3] * m11 + y;
			vertices[X4] = offset[X4] * m00 + offset[Y4] * m01 + x;
			vertices[Y4] = offset[X4] * m10 + offset[Y4] * m11 + y;
		}
	}
}
